using System.Collections.Generic;

using UnityEngine;

namespace RavingBots.MagicGestures.Utils.ObjectPooling
{
	/// <summary>
	///     A generic object pool.
	/// </summary>
	/// <remarks>
	///     Pooling is used for objects that are created frequently, but don't
	///     live very long (like the spell effects) to lower the allocation rate and avoid
	///     unnecessary collections of reusable instances, and to speed things up
	///     (as creating new instances can be expensive).
	/// </remarks>
	/// <typeparam name="T">The type of the component.</typeparam>
	/// <seealso cref="IPooledObject" />
	public class GameObjectPool<T> : IObjectPool
		where T : MonoBehaviour, IPooledObject
	{
		/// <summary>
		///     The prefab used to create new instances.
		/// </summary>
		private readonly T _prefab;
		/// <summary>
		///     The queue of cached instances.
		/// </summary>
		private readonly Queue<T> _queue = new Queue<T>();

		/// <summary>
		///     The scene container that keeps the created objects.
		/// </summary>
		private Transform _container;

		/// <inheritdoc cref="_container" />
		private Transform Container
		{
			get
			{
				if (!_container)
				{
					_container = new GameObject(string.Format("Pool: {0}", _prefab.gameObject.name)).transform;
					_container.parent = GameObjectPoolRoot.AutoInstance.transform;
					_container.gameObject.SetActive(false);
				}

				return _container;
			}
		}

		/// <summary>
		///     Create a new pool with the given prefab and initial size.
		/// </summary>
		public GameObjectPool(T prefab, int precache = 0)
		{
			Debug.Assert(prefab);

			_prefab = prefab;

			Precache(precache);
		}

		/// <summary>
		///     Prepare some instances before they're actually needed.
		/// </summary>
		public void Precache(int precache)
		{
			var precached = new List<T>();
			for (var i = 0; i < precache; i++)
				precached.Add(TakeInstance());

			foreach (var obj in precached)
				RevokeInstance(obj);
		}

		/// <summary>
		///     Take an instance out of the pool.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Instances taken out of the pool should not be destroyed, but
		///         rather returned to the pool using <see cref="RevokeInstance" />.
		///     </note>
		/// </remarks>
		/// <returns>A pooled instance.</returns>
		public T TakeInstance()
		{
			if (_queue.Count > 0)
			{
				var result = _queue.Dequeue();
				result.transform.parent = null;
				result.Queued = false;
				return result;
			}
			else
			{
				var inst = Object.Instantiate(_prefab.gameObject);
				inst.name = _prefab.gameObject.name;

				var result = inst.GetComponent<T>();
				result.gameObject.SetActive(false);
				result.Pool = this;
				result.Queued = false;
				return result;
			}
		}

		/// <summary>
		///     Return an instance to the pool.
		/// </summary>
		public void RevokeInstance(IPooledObject obj)
		{
			var o = (T)obj;

			if (o.gameObject.activeSelf)
				o.gameObject.SetActive(false);

			o.transform.parent = Container;
			_queue.Enqueue(o);
			o.Queued = true;
		}
	}
}
