using UnityEngine;

namespace RavingBots.MagicGestures.Utils.ObjectPooling
{
	/// <summary>
	///     The base class for pooled objects.
	/// </summary>
	/// <remarks>
	///     Contains the shared code for all pooled objects.
	/// </remarks>
	/// <seealso cref="GameObjectPool{T}" />
	public abstract class PooledGameObject : MonoBehaviour, IPooledObject
	{
		/// <inheritdoc />
		public bool Queued { get; set; }
		/// <inheritdoc />
		public IObjectPool Pool { get; set; }

		/// <inheritdoc cref="ResetState" />
		protected virtual void Awake()
		{
			ResetState();
		}

		/// <summary>
		///     Reset this instance to the pristine state.
		/// </summary>
		protected abstract void ResetState();

		/// <summary>
		///     Reset this instance and then return it to its pool.
		/// </summary>
		/// <seealso cref="ResetState"/>
		public void Revoke()
		{
			ResetState();
			Pool.RevokeInstance(this);
		}
	}
}
