using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.Game.Magic;

using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace RavingBots.MagicGestures.Controller
{
	/// <summary>
	///     The component responsible for the visual effects of casting spells:
	///     inflating the model, and updating the preview bubble.
	/// </summary>
	public class SpellCaster : MonoBehaviour
	{
		/// <summary>
		///     <see langword="true" /> if the effects are loaded.
		/// </summary>
		/// <seealso cref="LoadSpell" />
		public bool IsLoaded
		{
			get { return _fireEffect != null; }
		}

		/// <summary>
		///     The duration of the deflation animation.
		/// </summary>
		public float ClipScaleDuration = 0.2f;
		/// <summary>
		///     The rotation speed of the preview object.
		/// </summary>
		public float ClipRotationSpeed = 100f;

		/// <summary>
		///     The parent transform of the preview bubble.
		/// </summary>
		[SerializeField]
		protected Transform AnimatedClip;
		/// <summary>
		///     The transform of the preview bubble.
		/// </summary>
		[SerializeField]
		protected Transform PreviewAnchor;

		/// <summary>
		///     <see langword="true" /> if user is pressing the trigger.
		/// </summary>
		/// <seealso cref="SetPressed" />
		private bool _pressed;

		/// <inheritdoc cref="_pressed" />
		public bool Pressed
		{
			get { return _pressed; }
			set
			{
				if ((_pressed == value) || !IsLoaded)
					return;

				_pressed = value;

				SetPressed(_pressed);
			}
		}

		/// <summary>
		///     The pell effect being fired from the tip.
		/// </summary>
		private MagicEffect _fireEffect;
		/// <summary>
		///     The spell effect being previewed in the bubble.
		/// </summary>
		private MagicEffect _previewEffect;

		/// <summary>
		///     The initial scale of the preview bubble.
		/// </summary>
		private Vector3 _clipInitScale;
		/// <summary>
		///     The progress of the deflation animation.
		/// </summary>
		private float _clipScale;

		/// <summary>
		///     Save the initial scale of the preview bubble and hide it.
		/// </summary>
		protected void Awake()
		{
			_clipInitScale = AnimatedClip.localScale;
			AnimatedClip.localScale = Vector3.zero;
		}

		/// <summary>
		///     Update the animation progress.
		/// </summary>
		protected void Update()
		{
			AnimatedClip.localRotation = Quaternion.Euler(0f, Time.time * ClipRotationSpeed, 0f);

			var delta = (IsLoaded && !Pressed ? 1f : -1f) * Time.deltaTime / ClipScaleDuration;
			var clipScale = Mathf.Clamp01(_clipScale + delta);

			if (_clipScale != clipScale)
			{
				_clipScale = clipScale;
				AnimatedClip.localScale = _clipScale * _clipInitScale;

				if ((delta < 0f) && (_clipScale == 0f))
					ClearPreview();
			}
		}

		/// <summary>
		///     Load the given spell.
		/// </summary>
		public bool LoadSpell(int id, Color color)
		{
			ClearSpell();
			ClearPreview();

			var game = MagicGame.Instance;

			_fireEffect = game.CreateMagicEffect(id);
			_previewEffect = game.CreateMagicEffect(id);

			if (!_fireEffect || !_previewEffect)
				return false;

			_fireEffect.Color = color;
			_previewEffect.Color = color;

			_fireEffect.Setup(transform, false);
			_previewEffect.Setup(PreviewAnchor, true);

			return true;
		}

		/// <summary>
		///     Clear the spell being fired.
		/// </summary>
		public void ClearSpell()
		{
			if (_fireEffect)
			{
				_fireEffect.Revoke();
				_fireEffect = null;
			}

			_pressed = false;
		}

		/// <summary>
		///     Clear the spell being previewed.
		/// </summary>
		private void ClearPreview()
		{
			if (_previewEffect)
			{
				_previewEffect.Revoke();
				_previewEffect = null;
			}
		}

		/// <summary>
		///     Update the pressed state.
		/// </summary>
		private void SetPressed(bool pressed)
		{
			if (pressed)
			{
				if (_fireEffect)
					_fireEffect.OnPress();
			}
			else
			{
				if (_fireEffect)
				{
					_fireEffect.OnRelease();
					_fireEffect = null;
				}
			}
		}
	}
}
