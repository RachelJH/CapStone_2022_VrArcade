using RavingBots.MagicGestures.Utils;
using RavingBots.MagicGestures.Utils.Geometry;

using UnityEngine;

namespace RavingBots.MagicGestures.Game.Magic
{
	/// <summary>
	///     The component implementing the bullets that can be charged
	///     by holding the cast button.
	/// </summary>
	public abstract class ChargedBullet : MagicEffect
	{
		/// <summary>
		///     The current charge of this bullet.
		/// </summary>
		/// <seealso cref="SetCharge" />
		private float _currentCharge;

		/// <inheritdoc cref="_currentCharge" />
		public float CurrentCharge
		{
			get { return _currentCharge; }
			protected set
			{
				_currentCharge = value;

				SetCharge(_currentCharge);
			}
		}

		/// <summary>
		///     The bullet object associated with this effect.
		/// </summary>
		[SerializeField]
		protected Bullet Bullet;

		/// <summary>
		///     The duration of the charging effect.
		/// </summary>
		[SerializeField]
		protected float ChargeDuration = 4f;
		/// <summary>
		///     The minimum charge of the bullet.
		/// </summary>
		[SerializeField]
		protected float MinCharge = 0.05f;
		/// <summary>
		///     The time before the launched bullet autodestructs.
		/// </summary>
		[SerializeField]
		protected float AutodestructTime = 4f;

		/// <summary>
		///     The sound played while the bullet is being charged.
		/// </summary>
		[SerializeField]
		protected AudioSource ChargeSound;
		/// <summary>
		///     The sound played when the bullet is fired.
		/// </summary>
		[SerializeField]
		protected AudioSource FireSound;
		/// <summary>
		///     The scaling factor of the bullet charge, used to calculate
		///     the pitch of the firing sound.
		/// </summary>
		[SerializeField]
		protected float ChargeToFirePitch = 0.5f;
		/// <summary>
		///     The sound played while the bullet is flying.
		/// </summary>
		[SerializeField]
		protected AudioSource FlySound;
		/// <summary>
		///     The scaling factor of the bullet charge, used to calculate
		///     the pitch of the flying sound.
		/// </summary>
		[SerializeField]
		protected float ChargeToFlyPitch = 0.5f;

		/// <summary>
		///     The saved transform state of the bullet object.
		/// </summary>
		protected TransformData BulletTransform { get; private set; }

		/// <summary>
		///     <see langword="true" /> if the bullet has been fired.
		/// </summary>
		protected bool Fired { get; private set; }
		/// <summary>
		///     <see langword="true" /> if the bullet has autodestructed.
		/// </summary>
		protected bool AutoDestructed { get; private set; }

		/// <inheritdoc />
		protected override void Awake()
		{
			BulletTransform = Bullet.transform;

			base.Awake();
		}

		/// <inheritdoc />
		protected override void ResetState()
		{
			base.ResetState();

			Fired = false;
			AutoDestructed = false;

			ChargeSound.SafeStop();
			FireSound.SafeStop();
			FlySound.SafeStop();

			Bullet.gameObject.SetActive(false);
			Bullet.Frozen = true;
			Bullet.transform.Set(BulletTransform);

			CurrentCharge = 0f;
		}

		/// <summary>
		///     Called when the charge state of this bullet changes.
		/// </summary>
		protected virtual void SetCharge(float charge)
		{
		}

		/// <inheritdoc />
		public override void OnPress()
		{
			base.OnPress();

			Bullet.gameObject.SetActive(true);
			ChargeSound.SafePlay();
		}

		/// <inheritdoc />
		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			if (Pressed)
				Charge();

			if (!Alive)
				return;

			if (CurrentCharge < MinCharge)
				Charge();

			if (!Fired && (CurrentCharge >= MinCharge))
				Fire();

			if (!AutoDestructed && (Time.fixedTime >= ReleaseTime + AutodestructTime))
				AutoDestruct();
		}

		/// <summary>
		///     Update the charge state of this bullet.
		/// </summary>
		private void Charge()
		{
			CurrentCharge = Mathf.Clamp01(CurrentCharge + Time.fixedDeltaTime / ChargeDuration);
		}

		/// <summary>
		///     Called when the bullet is fired.
		/// </summary>
		protected virtual void Fire()
		{
			Fired = true;

			ChargeSound.SafeStop();
			FireSound.SafePlay(null, 1f, Mathf.Lerp(1f / ChargeToFirePitch, ChargeToFirePitch, CurrentCharge));
			FlySound.SafePlay(null, 1f, Mathf.Lerp(1f / ChargeToFlyPitch, ChargeToFlyPitch, CurrentCharge));

			Bullet.Fire();
		}

		/// <summary>
		///     Called when the bullet autodestructs.
		/// </summary>
		protected virtual void AutoDestruct()
		{
			AutoDestructed = true;

			FlySound.SafeStop();

			Bullet.gameObject.SetActive(false);
		}
	}
}
