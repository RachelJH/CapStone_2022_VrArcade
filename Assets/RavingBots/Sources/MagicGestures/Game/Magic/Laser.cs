using RavingBots.MagicGestures.Utils;

using UnityEngine;

namespace RavingBots.MagicGestures.Game.Magic
{
	/// <summary>
	///     The component implementing the inflatable capsule bullets.
	/// </summary>
	public class Laser : ChargedBullet
	{
		/// <summary>
		///     The duration of the shrink animation.
		/// </summary>
		[SerializeField]
		protected float ShrinkDuration = 0.2f;
		/// <summary>
		///     The renderers used by the effect.
		/// </summary>
		[SerializeField]
		protected Renderer[] Lines;

		/// <summary>
		///     The line renderer used.
		/// </summary>
		private LineRenderer _bulletLine;
		/// <summary>
		///     The collider used.
		/// </summary>
		private CapsuleCollider _bulletCapsule;

		/// <summary>
		///     The mass of the capsule.
		/// </summary>
		private float _mass;
		/// <summary>
		///     The starting Y position of the bullet.
		/// </summary>
		private float _startY;
		/// <summary>
		///     The ending Y position of the bullet.
		/// </summary>
		private float _endY;
		/// <summary>
		///     The height of the bullet capsule.
		/// </summary>
		private float _height;
		/// <summary>
		///     The Y position of the capsule center.
		/// </summary>
		private float _posY;
		/// <summary>
		///     The timestamp of previous collision with another body.
		/// </summary>
		private float _collisionTime = -1f;

		/// <inheritdoc />
		protected override void Awake()
		{
			Bullet.OnCollision += OnBulletCollision;

			_mass = Bullet.Rigidbody.mass;
			_bulletLine = Bullet.GetComponent<LineRenderer>();
			_bulletCapsule = Bullet.GetComponent<CapsuleCollider>();

			_startY = _bulletLine.GetPosition(0).y;
			_endY = _bulletLine.GetPosition(1).y;

			_posY = _bulletCapsule.center.y;
			_height = _bulletCapsule.height;

			base.Awake();
		}

		/// <inheritdoc />
		protected override void ResetState()
		{
			base.ResetState();

			_collisionTime = -1f;
		}

		/// <inheritdoc />
		protected override void SetCharge(float charge)
		{
			base.SetCharge(charge);

			charge = Mathf.Sqrt(charge);

			Bullet.Rigidbody.mass = charge * _mass;
			_bulletLine.SetPosition(1, new Vector3(0f, Mathf.Lerp(_startY, _endY, charge), 0f));
			_bulletCapsule.height = Mathf.Lerp(0f, _height, charge);
			_bulletCapsule.center = new Vector3(0f, _startY + Mathf.Lerp(0f, _posY - _startY, charge), 0f);
		}

		/// <inheritdoc />
		protected override void SetColor(Color color)
		{
			foreach (var line in Lines)
			{
				line.material.color = color;
				line.material.SetEmission(color);
			}
		}

		/// <summary>
		///     Called when the bullet collides with something.
		/// </summary>
		private void OnBulletCollision(Collision c)
		{
			if (_collisionTime < 0)
			{
				_collisionTime = Time.fixedTime;

				Bullet.Frozen = true;
				Bullet.CollisionSound.SafePlay(null, 1f);

				FlySound.SafeStop();
			}
		}

		/// <inheritdoc />
		protected override void Update()
		{
			base.Update();

			if (_collisionTime >= 0f)
			{
				CurrentCharge = Mathf.Clamp01(CurrentCharge - Time.deltaTime / ShrinkDuration);

				if ((CurrentCharge <= 0f) && !AutoDestructed)
					AutoDestruct();
			}
		}
	}
}
