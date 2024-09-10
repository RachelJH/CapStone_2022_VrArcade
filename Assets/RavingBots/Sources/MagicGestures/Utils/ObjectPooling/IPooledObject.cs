namespace RavingBots.MagicGestures.Utils.ObjectPooling
{
	/// <summary>
	///     The interface that all pooled objects must implement.
	/// </summary>
	public interface IPooledObject
	{
		/// <summary>
		///     <see langword="true" /> if the object is currently
		///     unused and waiting in the pool.
		/// </summary>
		bool Queued { get; set; }
		/// <summary>
		///     The pool this object belongs to. Objects cannot be shared
		///     between different pools.
		/// </summary>
		IObjectPool Pool { get; set; }
	}
}
