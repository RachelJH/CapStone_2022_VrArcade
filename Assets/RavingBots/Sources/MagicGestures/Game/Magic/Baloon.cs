using RavingBots.MagicGestures.Utils;
using RavingBots.MagicGestures.Utils.Geometry;

using UnityEngine;

namespace RavingBots.MagicGestures.Game.Magic
{
	/// <summary>
	///     The component implementing the exploding inflatable sphere bullets.
	/// </summary>
	/// <seealso cref="ChargedBullet" />
	/// <seealso cref="BulletLauncher" />
	public class Baloon : ChargedBullet
	{
		/// <summary>
		///     The radius of the explosion.
		/// </summary>
		[SerializeField]
		protected float ExplosionRadius = 5f;
		/// <summary>
		///     The strength of the explosion.
		/// </summary>
		[SerializeField]
		protected float ExplosionPower = 1000f;

		/// <summary>
		///     The trail effect used.
		/// </summary>
		[SerializeField]
		protected ParticleSystem Trail;
		/// <summary>
		///     The detonation effect used.
		/// </summary>
		[SerializeField]
		protected ParticleSystem Detonation;

		/// <summary>
		///     The mesh renderers that are colored according
		///     to configured spell preferences.
		/// </summary>
		[SerializeField]
		protected MeshRenderer[] ColoredMesh;
		/// <summary>
		///     The particle systems that are colored according
		///     to configured spell preferences.
		/// </summary>
		[SerializeField]
		protected ParticleSystem[] ColoredParticles;
		/// <summary>
		///     The configured color lighten factor.
		/// </summary>
		[SerializeField]
		protected float ColorLighten = 0.5f;

		/// <summary>
		///     The sound played on explosion.
		/// </summary>
		[SerializeField]
		protected AudioSource DetonationSound;
		/// <summary>
		///     The scaling factor of the bullet charge, used to calculate
		///     the pitch of the explosion sound.
		/// </summary>
		[SerializeField]
		protected float ChargeToDetonationPitch = 0.5f;

		/// <summary>
		///     The saved transform of the trail effect.
		/// </summary>
		private TransformData _trailTransform;
		/// <summary>
		///     The saved transform of the detonation effect.
		/// </summary>
		private TransformData _detonationTransform;

		/// <summary>
		///     The emission parameters of the trail.
		/// </summary>
		private ParticleSystem.EmissionModule _trailEmission;
		/// <summary>
		///     The emission rate of the trail.
		/// </summary>
		private float _emissionRate;

		/// <inheritdoc />
		protected override void Awake()
		{
			_trailTransform = Trail.transform;
			_detonationTransform = Detonation.transform;

			_trailEmission = Trail.emission;
			_emissionRate = _trailEmission.rateOverTimeMultiplier;

			base.Awake();
		}

		/// <inheritdoc />
		protected override void ResetState()
		{
			base.ResetState();

			Trail.Stop();
			_trailEmission.rateOverTimeMultiplier = _emissionRate;
			Trail.transform.Set(_trailTransform);

			Detonation.Stop();
			Detonation.transform.Set(_detonationTransform);
			DetonationSound.SafeStop();
		}

		/// <inheritdoc />
		protected override void SetCharge(float charge)
		{
			base.SetCharge(charge);

			Bullet.transform.localScale = Mathf.Sqrt(charge) * BulletTransform.LocalScale;
		}

		/// <inheritdoc />
		protected override void SetColor(Color color)
		{
			base.SetColor(color);

			color = Color.Lerp(color, Color.white, ColorLighten);

			foreach (var particle in ColoredParticles)
				particle.GetComponent<ParticleSystemRenderer>().material.SetEmission(color);

			foreach (var mesh in ColoredMesh)
				mesh.material.color = color;
		}

		/// <inheritdoc />
		protected override void Fire()
		{
			base.Fire();

			Trail.Play();
		}

		/// <inheritdoc />
		protected override void AutoDestruct()
		{
			Trail.transform.parent = null;
			_trailEmission.rateOverTimeMultiplier = 0f;

			Detonation.transform.parent = null;
			Detonation.Play();
			DetonationSound.SafePlay(null, 1f, Mathf.Lerp(1f / ChargeToDetonationPitch, ChargeToDetonationPitch, CurrentCharge));

			base.AutoDestruct();

			PhysicsUtils.Explode(Bullet.transform.position, ExplosionRadius * CurrentCharge, ExplosionPower * CurrentCharge);
		}
	}
}
