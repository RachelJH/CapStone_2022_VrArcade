using System.Collections.Generic;
using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.UI;
using RavingBots.MagicGestures.Utils;
using RavingBots.MagicGestures.Utils.ObjectPooling;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     The component responsible for rendering the visual gesture trail.
	/// </summary>
	/// <seealso cref="PooledGameObject" />
	[RequireComponent(typeof(LineRenderer))]
	public class GestureTrail : PooledGameObject
	{
		/// <summary>
		///     The duration of the trail fade.
		/// </summary>
		public float FadeDuration = 0.5f;

		/// <summary>
		///     The duration of the trail morph.
		/// </summary>
		public float MorphDuration = 0.2f;

		/// <summary>
		///     The line renderer used for the trail.
		/// </summary>
		public LineRenderer LineRenderer { get; private set; }

		/// <summary>
		///     The trail's line width.
		/// </summary>
		private float _lineWidth;

		/// <summary>
		///     The trail's line color.
		/// </summary>
		private Color _lineColor;

		/// <summary>
		///     The trail's layer.
		/// </summary>
		private int _layer;

		/// <summary>
		///     The progress of the morph effect.
		/// </summary>
		private float _morph;

		/// <summary>
		///     The progress of the fade effect.
		/// </summary>
		private float _fade;

		/// <summary>
		///     <see langword="true" /> if user has released the button.
		/// </summary>
		private bool _released;

		/// <summary>
		///     The target positions of the trail points, used in the morph effect.
		/// </summary>
		private Vector3[] _morphPoints;

		/// <summary>
		///     The target color of the trail, used in the morph effect.
		/// </summary>
		private Color _morphColor;

		/// <summary>
		///     Grab configuration from the <c>LineRenderer</c>.
		/// </summary>
		protected override void Awake()
		{
			LineRenderer = GetComponent<LineRenderer>();
			_lineWidth = LineRenderer.widthMultiplier;
			_lineColor = LineRenderer.material.GetEmission();
			_layer = LineRenderer.gameObject.layer;

			base.Awake();
		}

		/// <summary>
		///     Set points on the trail.
		/// </summary>
		public void SetLine(List<Vector3> positions)
		{
			LineRenderer.positionCount = positions.Count + 1;

			for (var i = 0; i < positions.Count; i++)
				LineRenderer.SetPosition(i, positions[i]);
		}

		/// <summary>
		///     Update the most recent point of the trail.
		/// </summary>
		public void SetTip(Vector3 position)
		{
			LineRenderer.SetPosition(LineRenderer.positionCount - 1, position);
		}

		/// <summary>
		///     Set the morphing parameters.
		/// </summary>
		/// <returns>
		///     <see langword="false" /> if <paramref name="points" /> has a different length than
		///     there are points on the trail.
		/// </returns>
		public bool SetMorph(ArrayWrapper pointArray, Color color)
		{
			if (pointArray.Length() != LineRenderer.positionCount)
				return false;

			var points = new List<Vector3>();


			for (var j = 0; j < pointArray.Length(); j++) points.Add(pointArray[j]);

			_morphPoints = points.ToArray();
			_morphColor = color;

			return true;
		}

		/// <summary>
		///     Called when the user releases the button.
		/// </summary>
		public void Release()
		{
			_released = true;
		}

		/// <summary>
		///     Update the visual effects.
		/// </summary>
		protected void Update()
		{
			if (!_released)
				return;

			if (UpdateMorph())
				return;

			if (!UpdateFade())
				Revoke();
		}

		/// <summary>
		///     Update the layer of <see cref="LineRenderer" />.
		/// </summary>
		/// <remarks>
		///     When the menu is shown, the trail is temporarily put on the UI layer.
		/// </remarks>
		protected void LateUpdate()
		{
			LineRenderer.gameObject.layer = MagicMenu.Instance.Shown ? PhysicsUtils.LayerUI : _layer;
		}

		/// <summary>
		///     Update the morph effect.
		/// </summary>
		private bool UpdateMorph()
		{
			if ((_morphPoints == null) || (_morph == 1f))
				return false;

			var morph = Mathf.Clamp01(_morph + Time.deltaTime / MorphDuration);

			if (_morph != morph)
			{
				_morph = morph;

				var m = _morph * _morph;

				if (_morphPoints.Length == 2 * LineRenderer.positionCount)
					for (var i = 0; i < _morphPoints.Length; i++)
						LineRenderer.SetPosition(i / 2,
							Vector3.Lerp(LineRenderer.GetPosition(i / 2), _morphPoints[i], m));
				else
					for (var i = 0; i < _morphPoints.Length; i++)
						LineRenderer.SetPosition(i, Vector3.Lerp(LineRenderer.GetPosition(i), _morphPoints[i], m));

				LineRenderer.material.SetEmission(Color.Lerp(_lineColor, _morphColor, m));
			}

			return true;
		}

		/// <summary>
		///     Update the fade effect.
		/// </summary>
		private bool UpdateFade()
		{
			if (_fade == 0f)
				return false;

			var fade = Mathf.Clamp01(_fade - Time.deltaTime / FadeDuration);

			if (_fade != fade)
			{
				_fade = fade;

				LineRenderer.widthMultiplier = Mathf.Sqrt(_fade) * _lineWidth;
			}

			return true;
		}

		/// <inheritdoc />
		protected override void ResetState()
		{
			LineRenderer.positionCount = 0;
			LineRenderer.widthMultiplier = _lineWidth;
			LineRenderer.material.SetEmission(_lineColor);
			LineRenderer.gameObject.layer = _layer;

			_morph = 0f;
			_fade = 1f;
			_released = false;

			_morphPoints = null;
			_morphColor = _lineColor;
		}
	}
}
