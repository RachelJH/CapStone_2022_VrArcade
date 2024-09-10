namespace RavingBots.MagicGestures.Utils.ObjectPooling
{
	/// <summary>
	///     The root scene container for the pools.
	/// </summary>
	public class GameObjectPoolRoot : MonoSingleton<GameObjectPoolRoot>
	{
		protected override void Awake()
		{
			base.Awake();

			name = "Pools";
			gameObject.SetActive(false);
		}
	}
}
