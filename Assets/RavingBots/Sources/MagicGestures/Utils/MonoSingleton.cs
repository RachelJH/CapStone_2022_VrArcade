using UnityEngine;

namespace RavingBots.MagicGestures.Utils
{
	/// <summary>
	///     The convenience base class for singleton objects.
	/// </summary>
	/// <remarks>
	///     This class is used as a base for objects that must
	///     not be duplicated on the scene. For convenience it also
	///     provides an easy access to the only instance.
	/// </remarks>
	/// <typeparam name="T">The type of the object.</typeparam>
	public class MonoSingleton<T> : MonoBehaviour
		where T : MonoBehaviour
	{
		/// <summary>
		///     The instance of the object, <see langword="null" /> if not created yet.
		/// </summary>
		public static T Instance { get; private set; }

		/// <summary>
		///     The instance of the object.
		/// </summary>
		/// <remarks>
		///     If no objects of this type exist, one will be created.
		/// </remarks>
		public static T AutoInstance
		{
			get
			{
				if (!Instance)
					new GameObject(string.Format("{0} Singleton", typeof(T).Name), typeof(T));

				return Instance;
			}
		}

		/// <summary>
		///     Set <see cref="Instance" />.
		/// </summary>
		protected virtual void Awake()
		{
			Debug.AssertFormat(!Instance, "One instance of {0} Singleton is already present", typeof(T).Name);

			Instance = this as T;
		}

		/// <summary>
		///     Clear registered <see cref="Instance" />.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Instance = null;
		}
	}
}
