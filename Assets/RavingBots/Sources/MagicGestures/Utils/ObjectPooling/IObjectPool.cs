namespace RavingBots.MagicGestures.Utils.ObjectPooling
{
	/// <summary>
	///     The interface to an object pool.
	/// </summary>
	/// <remarks>
	///     This doesn't contain <c>TakeInstance</c> as
	///     it's a generic method that depends on the pool type.
	/// </remarks>
	public interface IObjectPool
	{
		/// <summary>
		///     Return an instance to the pool.
		/// </summary>
		void RevokeInstance(IPooledObject obj);
	}
}
