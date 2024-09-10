using RavingBots.MagicGestures.Game;
using RavingBots.MagicGestures.Game.Magic;

using UnityEngine;

namespace RavingBots.MagicGestures.UI.Elements
{
	/// <summary>
	///     A bubble button to select the spell effect.
	/// </summary>
	public class EffectSlot : BubbleButton
	{
		/// <summary>
		///     The rotation speed of the effect preview.
		/// </summary>
		public float RotationSpeed = 100f;

		/// <summary>
		///     The effect to show.
		/// </summary>
		public MagicEffect MagicEffect { get; private set; }

		/// <summary>
		///     The index of the effect this button represents.
		/// </summary>
		/// <seealso cref="MagicGame.MagicEffectPrefabs" />
		/// <seealso cref="SetEffect" />
		private int _effectId = -1;

		/// <inheritdoc cref="_effectId" />
		public int EffectId
		{
			get { return _effectId; }
			set
			{
				if (_effectId == value)
					return;

				_effectId = value;

				SetEffect(_effectId);
			}
		}

		/// <summary>
		///     The anchor of the effect preview.
		/// </summary>
		/// <seealso cref="M:RavingBots.MagicGestures.Game.Magic.MagicEffect.Setup(UnityEngine.Transform,System.Boolean)" />
		[SerializeField]
		protected Transform Container;

		/// <summary>
		///     Called when the effect index changes.
		/// </summary>
		protected void SetEffect(int id)
		{
			if (MagicEffect)
				MagicEffect.Revoke();

			MagicEffect = MagicGame.Instance.CreateMagicEffect(id);

			if (MagicEffect)
			{
				MagicEffect.Setup(Container, true);
				MagicEffect.PreviewLayer = gameObject.layer;
			}
		}

		/// <inheritdoc />
		protected override void Update()
		{
			base.Update();

			if (Selected)
				Container.localRotation = Quaternion.Euler(0f, Time.deltaTime * RotationSpeed, 0f) * Container.localRotation;
		}
	}
}
