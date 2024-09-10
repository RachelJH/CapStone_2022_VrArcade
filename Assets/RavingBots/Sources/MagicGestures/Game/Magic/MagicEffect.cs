using RavingBots.MagicGestures.Utils.Geometry;
using RavingBots.MagicGestures.Utils.ObjectPooling;

using UnityEngine;

namespace RavingBots.MagicGestures.Game.Magic
{
	/// <summary>
	///     The base class for the magical effects.
	/// </summary>
	/// <remarks>
	///     The effects are configured and stored in prefabs.
	/// </remarks>
	/// <seealso cref="MagicGame.MagicEffectPrefabs" />
	public abstract class MagicEffect : PooledGameObject
	{
		/// <summary>
		///     The effect lifetime.
		/// </summary>
		[SerializeField]
		protected float TimeToLive = 30f;
		/// <summary>
		///     The container of the effect's preview.
		/// </summary>
		[SerializeField]
		protected GameObject PreviewContainer;

		/// <summary>
		///     <see langword="true" /> if the trigger is pressed.
		/// </summary>
		protected bool Pressed { get; private set; }
		/// <summary>
		///     The imestamp of the trigger release.
		/// </summary>
		protected float ReleaseTime { get; private set; }

		/// <summary>
		///     <see langword="true" /> if the trigger has been released.
		/// </summary>
		protected bool Released
		{
			get { return ReleaseTime >= 0f; }
		}

		/// <summary>
		///     <see langword="true" /> if this effect hasn't expired yet.
		/// </summary>
		protected bool Alive
		{
			get { return Released && (Time.time <= ReleaseTime + TimeToLive); }
		}

		/// <summary>
		///     <see langword="true" /> if this effect is used as a preview.
		/// </summary>
		public bool Preview
		{
			get { return PreviewContainer.activeSelf; }
			set
			{
				if (Preview == value)
					return;

				PreviewContainer.SetActive(value);
			}
		}

		/// <summary>
		///     The layer used when this effect is used as a preview.
		/// </summary>
		/// <seealso cref="SetPreviewLayer" />
		private int _previewLayer;

		/// <inheritdoc cref="_previewLayer" />
		public int PreviewLayer
		{
			get { return _previewLayer; }
			set
			{
				if (_previewLayer == value)
					return;

				_previewLayer = value;

				SetPreviewLayer(_previewLayer);
			}
		}

		/// <summary>
		///     The color of this effect.
		/// </summary>
		/// <seealso cref="SetColor" />
		private Color _color = Color.white;

		/// <inheritdoc cref="_color" />
		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color == value)
					return;

				_color = value;

				SetColor(_color);
			}
		}

		/// <summary>
		///     The saved initial state of this effect's transform.
		/// </summary>
		private TransformData _initialTransform;
		/// <summary>
		///     The anchor of this effect.
		/// </summary>
		private Transform _anchor;
		/// <summary>
		///     The MeshRenderers involved in the preview.
		/// </summary>
		private MeshRenderer[] _previewRenderers;

		/// <inheritdoc />
		protected override void Awake()
		{
			_initialTransform = transform;

			_previewRenderers = PreviewContainer.GetComponentsInChildren<MeshRenderer>();
			_previewLayer = PhysicsUtils.LayerDefault;
			SetPreviewLayer(_previewLayer);

			base.Awake();

			SetColor(_color);
		}

		/// <summary>
		///     Configure this effect.
		/// </summary>
		/// <param name="anchor">The anchor to use.</param>
		/// <param name="preview"><see langword="true" /> if this effect will be used as a preview.</param>
		public void Setup(Transform anchor, bool preview)
		{
			if (!preview)
			{
				_anchor = anchor;

				MoveToAnchor();
			}
			else
			{
				transform.parent = anchor;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
			}

			Preview = preview;
			gameObject.SetActive(true);
		}

		/// <inheritdoc />
		protected override void ResetState()
		{
			transform.Set(_initialTransform);
			PreviewLayer = PhysicsUtils.LayerDefault;

			Pressed = false;
			ReleaseTime = -1f;
			Preview = false;
			_anchor = null;
		}

		/// <summary>
		///     Called when the preview layer of this effect changes.
		/// </summary>
		private void SetPreviewLayer(int layer)
		{
			foreach (var r in _previewRenderers)
				r.gameObject.layer = layer;
		}

		/// <summary>
		///     Called when the color of the effect changes.
		/// </summary>
		protected virtual void SetColor(Color color)
		{
		}

		/// <summary>
		///     Update the effect state.
		/// </summary>
		protected virtual void Update()
		{
			if (Released && !Alive)
				Revoke();
		}

		/// <summary>
		///     Move the effect to the configured anchor.
		/// </summary>
		/// <seealso cref="_anchor" />
		private void MoveToAnchor()
		{
			if (_anchor)
			{
				transform.position = _anchor.position;
				transform.rotation = _anchor.rotation;
			}
		}

		/// <summary>
		///     Update the effect state.
		/// </summary>
		protected virtual void FixedUpdate()
		{
			MoveToAnchor();
		}

		/// <summary>
		///     Called when the user presses the trigger.
		/// </summary>
		public virtual void OnPress()
		{
			Pressed = true;
		}

		/// <summary>
		///     Called when the user releases the trigger.
		/// </summary>
		public virtual void OnRelease()
		{
			Pressed = false;
			ReleaseTime = Time.time;
		}
	}
}
