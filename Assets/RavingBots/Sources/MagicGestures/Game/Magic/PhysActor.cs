using RavingBots.MagicGestures.Utils;

using UnityEngine;

namespace RavingBots.MagicGestures.Game.Magic
{
	/// <summary>
	///     The base component for the physical actors.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class PhysActor : MonoBehaviour
	{
		/// <summary>
		///     The sound played on a collision.
		/// </summary>
		public AudioSource CollisionSound;
		/// <summary>
		///     The minimum collision velocity before the sound is played.
		/// </summary>
		[SerializeField]
		protected float CollisionSoundMinVelocity = 0.5f;
		/// <summary>
		///     The scaling factor, used to convert the collision velocity
		///     to the collision sound volume.
		/// </summary>
		[SerializeField]
		protected float CollisionVelocityToVolume = 0.25f;

		/// <summary>
		///     Called when this actor collides with something.
		/// </summary>
		protected virtual void OnCollisionEnter(Collision collision)
		{
			var collisionVelocity = Vector3.Dot(collision.relativeVelocity, collision.contacts[0].normal);

			CollisionSound.SafePlayCollision(null, collisionVelocity, CollisionSoundMinVelocity, CollisionVelocityToVolume);
		}
	}
}
