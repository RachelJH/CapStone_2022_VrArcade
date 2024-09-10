using RavingBots.MagicGestures.Utils;
using RavingBots.MagicGestures.Utils.Geometry;

using UnityEngine;

namespace RavingBots.MagicGestures.Game.Magic
{
	/// <summary>
	///     The component for the spells based on launching bullets.
	/// </summary>
	/// <remarks>
	///     The spells themselves are configured in prefabs.
	/// </remarks>
	public class BulletLauncher : MagicEffect
	{
		/// <summary>
		///     The number of bullets to launch.
		/// </summary>
		[SerializeField]
		protected int BulletCount = 5;
		/// <summary>
		///     The bullet prefab to use.
		/// </summary>
		[SerializeField]
		protected Bullet BulletPrefab;
		/// <summary>
		///     The interval between bullets.
		/// </summary>
		[SerializeField]
		protected float FireInterval = 0.2f;
		/// <summary>
		///     Teh configured color lighten factor.
		/// </summary>
		[SerializeField]
		protected float ColorLighten = 0.5f;

		/// <summary>
		///     The sound played when a bullet is launched.
		/// </summary>
		[SerializeField]
		protected AudioSource FireSound;

		/// <summary>
		///     The bullet instances.
		/// </summary>
		private Bullet[] _bullets;
		/// <summary>
		///     The mesh renderers that are colored according
		///     to configured spell preferences.
		/// </summary>
		private MeshRenderer[] _coloredMesh;
		/// <summary>
		///     The trail renderers that are colored according
		///     to configured spell preferences.
		/// </summary>
		private TrailRenderer[] _coloredTrail;

		/// <summary>
		///     The saved transform of the bullet.
		/// </summary>
		private TransformData _bulletTransform;
		/// <summary>
		///     The time since first bullet was launched.
		/// </summary>
		private float _firedTime;
		/// <summary>
		///     The number of bullets already launched.
		/// </summary>
		private int _firedCount;

		/// <inheritdoc />
		protected override void Awake()
		{
			_bulletTransform = BulletPrefab.transform;
			_bulletTransform.Parent = transform;

			CreateBullets();

			_coloredMesh = GetComponentsInChildren<MeshRenderer>();
			_coloredTrail = GetComponentsInChildren<TrailRenderer>();

			base.Awake();
		}

		/// <summary>
		///     Create the bullet instances.
		/// </summary>
		private void CreateBullets()
		{
			_bullets = new Bullet[BulletCount];
			for (var i = 0; i < _bullets.Length; i++)
				_bullets[i] = Instantiate(BulletPrefab, transform);
		}

		/// <inheritdoc />
		protected override void ResetState()
		{
			base.ResetState();

			_firedTime = -1f;
			_firedCount = 0;

			ResetBullets();

			FireSound.SafeStop();
		}

		/// <inheritdoc />
		protected override void SetColor(Color color)
		{
			base.SetColor(color);

			foreach (var trail in _coloredTrail)
				trail.material.SetEmission(color);

			color = Color.Lerp(color, Color.white, ColorLighten);

			foreach (var mesh in _coloredMesh)
				mesh.material.color = color;
		}

		/// <summary>
		///     Reset the bullet instances.
		/// </summary>
		private void ResetBullets()
		{
			foreach (var bullet in _bullets)
			{
				bullet.gameObject.SetActive(false);
				bullet.Frozen = true;
				bullet.transform.Set(_bulletTransform);
			}
		}

		/// <inheritdoc />
		public override void OnRelease()
		{
			base.OnRelease();

			Fire();
		}

		/// <summary>
		///     Fire another bullet.
		/// </summary>
		private void Fire()
		{
			_bullets[_firedCount++].Fire();
			_firedTime = Time.time;

			FireSound.SafePlay();
		}

		/// <summary>
		///     <see langword="true" /> if another bullet can be fired.
		/// </summary>
		private bool CanFire()
		{
			return Alive && (_firedCount < _bullets.Length) && (Time.time >= _firedTime + FireInterval);
		}

		/// <inheritdoc />
		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			if (CanFire())
				Fire();
		}
	}
}
