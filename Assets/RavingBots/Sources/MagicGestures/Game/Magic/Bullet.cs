using System;

using UnityEngine;

namespace RavingBots.MagicGestures.Game.Magic
{
	/// <summary>
	///     The component implementing the bullet physics.
	/// </summary>
	/// <seealso cref="BulletLauncher" />
	[RequireComponent(typeof(Rigidbody))]
	public class Bullet : PhysActor
	{
		/// <summary>
		///     The launch velocity of the bullet.
		/// </summary>
		[SerializeField]
		protected float LaunchVelocity = 10;
		/// <summary>
		///     The duration of the bullet's thrust.
		/// </summary>
		[SerializeField]
		protected float ThrustDuration = 2f;
		/// <summary>
		///     The strength of the bullet's thrust.
		/// </summary>
		[SerializeField]
		protected float ThrustPower = 100f;

		/// <summary>
		///     If <see langword="true" />, the gravity will not affect this bullet.
		/// </summary>
		[SerializeField]
		protected bool IgnoreGravity;

		/// <summary>
		///     Called when the bullet collides with another body.
		/// </summary>
		public event Action<Collision> OnCollision;

		/// <summary>
		///     The <c>Rigidbody</c> instance used.
		/// </summary>
		private Rigidbody _rigidbody;

		/// <inheritdoc cref="_rigidbody" />
		public Rigidbody Rigidbody
		{
			get { return _rigidbody ?? (_rigidbody = GetComponent<Rigidbody>()); }
		}

		/// <summary>
		///     If <see langword="true" />, the bullet will be frozen in place.
		/// </summary>
		public bool Frozen
		{
			get { return Rigidbody.isKinematic; }
			set
			{
				Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
				Rigidbody.isKinematic = value;
				Rigidbody.useGravity = !value && !IgnoreGravity;
			}
		}

		/// <summary>
		///     The current duration of the thrust.
		/// </summary>
		private float _thrustDuration;

		/// <summary>
		///     Launch the bullet.
		/// </summary>
		public void Fire()
		{
			Frozen = false;
			transform.parent = null;

			if (!gameObject.activeSelf)
				gameObject.SetActive(true);

			Rigidbody.AddForce(LaunchVelocity * transform.up, ForceMode.VelocityChange);
			_thrustDuration = ThrustDuration;
		}

		/// <summary>
		///     Apply the thrust force if necessary.
		/// </summary>
		protected void FixedUpdate()
		{
			if (_thrustDuration > 0f)
			{
				var t = Time.fixedDeltaTime;
				_thrustDuration -= t;

				Rigidbody.AddForce(t * ThrustPower * transform.up);
			}
		}

		/// <inheritdoc />
		protected override void OnCollisionEnter(Collision collision)
		{
			base.OnCollisionEnter(collision);

			if (OnCollision != null)
				OnCollision(collision);
		}
	}
}
