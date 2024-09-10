using RavingBots.MagicGestures.Controller;
using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.Utils;
using UnityEngine;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     A bubble button representing a slot in the spellbook.
	/// </summary>
	public class SpellSlot : BubbleButton
	{
		/// <summary>
		///     The preview rotation angle.
		/// </summary>
		public float PreviewWiggleAngle = 15f;

		/// <summary>
		///     The speed of preview rotation.
		/// </summary>
		public float PreviewWiggleSpeed = 1f;

		/// <summary>
		///     The density of the preview.
		/// </summary>
		/// <seealso cref="M:RavingBots.MagicGestures.Game.SpellData.CreatePreview(System.Single)" />
		public float PreviewPointsDensity = 10f;

		/// <summary>
		///     The spell to preview. May be <see langword="null" />.
		/// </summary>
		/// <seealso cref="SetSpellData" />
		private SpellData _spellData;

		/// <inheritdoc cref="_spellData" />
		public SpellData SpellData
		{
			get { return _spellData; }
			set
			{
				if (_spellData == value)
					return;

				_spellData = value;

				SetSpellData(_spellData);
			}
		}

		/// <summary>
		///     The gesture used to preview the spell. May be <see langword="null" />.
		/// </summary>
		public GestureData PreviewGesture { get; private set; }

		/// <summary>
		///     <see langword="true" /> if the spell is set and valid.
		/// </summary>
		public bool IsValid
		{
			get { return (SpellData != null) && SpellData.IsValid; }
		}

		/// <summary>
		///     The line renderer used.
		/// </summary>
		private LineRenderer[] _lineRenderer;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();

			_lineRenderer = GetComponentsInChildren<LineRenderer>();
		}

		/// <summary>
		///     Called when the previewed spell changes.
		/// </summary>
		private void SetSpellData(SpellData spellData)
		{
			if (IsValid)
			{
				PreviewGesture = spellData.CreatePreview(PreviewPointsDensity);
				var points = new Vector3[PreviewGesture.Points.Length][];

				for (var i = _lineRenderer.Length-1; i > PreviewGesture.Points.Length-1; i--)
				{
					_lineRenderer[i].positionCount = 0;
					_lineRenderer[i].SetPositions(new Vector3[0]);
				}

				for (var i = 0; i < PreviewGesture.Points.Length; i++)
				{
					points[i] = new Vector3[PreviewGesture.Points[i].Length()];
					for (var j = 0; j < PreviewGesture.Points[i].Length(); j++)
						points[i][j] = PreviewGesture.Points[i][j];
					_lineRenderer[i].positionCount = points[i].Length;
					_lineRenderer[i].SetPositions(points[i]);
					_lineRenderer[i].material.SetEmission(spellData.Color);
				}
			}
			else
				foreach (var lineRenderer in _lineRenderer)
					lineRenderer.positionCount = 0;
		}

		/// <inheritdoc />
		protected override void Update()
		{
			base.Update();

			_lineRenderer[0].transform.parent.localRotation = Quaternion.Euler(
				0,
				PreviewWiggleAngle * Mathf.Cos(Time.time * PreviewWiggleSpeed),
				0f);
		}
	}
}
