using System;
using System.Collections;
using System.Collections.Generic;

namespace RavingBots.MagicGestures.Utils
{
	/// <summary>
	///     An <see cref="IList{T}" /> implementation that
	///     notifies when new elements are added or removed.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	public class MonitoredList<T> : IList<T>
	{
		/// <summary>
		///     The items.
		/// </summary>
		public readonly List<T> List;
		/// <summary>
		///     The delegate called when a new item is added.
		/// </summary>
		public readonly Action<T> OnAdded;
		/// <summary>
		///     The delegate called when an item is removed.
		/// </summary>
		public readonly Action<T> OnRemoved;
		/// <summary>
		///     The delegate called when an item is moved from one index to another.
		/// </summary>
		/// <seealso cref="Move" />
		public readonly Action<T> OnMoved;

		/// <inheritdoc />
		public int Count
		{
			get { return List.Count; }
		}

		/// <inheritdoc />
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		///     Construct a new list with the given initial items and delegates.
		/// </summary>
		public MonitoredList(List<T> list, Action<T> onAdded, Action<T> onRemoved, Action<T> onMoved)
		{
			List = list;
			OnAdded = onAdded;
			OnRemoved = onRemoved;
			OnMoved = onMoved;
		}

		/// <inheritdoc />
		public T this[int i]
		{
			get { return List[i]; }
			set { List[i] = value; }
		}

		/// <summary>
		///     Check if the given index exists in this list.
		/// </summary>
		public bool ContainsIndex(int index)
		{
			return (index >= 0) && (index < List.Count);
		}

		/// <inheritdoc />
		public void Add(T item)
		{
			List.Add(item);

			if (OnAdded != null)
				OnAdded(item);
		}

		/// <summary>
		///     Add multiple items to this list.
		/// </summary>
		public void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
				Add(item);
		}

		/// <inheritdoc />
		public void CopyTo(T[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(T item)
		{
			var result = List.Remove(item);

			if (OnRemoved != null)
				OnRemoved(item);

			return result;
		}

		/// <inheritdoc />
		public int IndexOf(T item)
		{
			return List.IndexOf(item);
		}

		/// <inheritdoc />
		public void Insert(int index, T item)
		{
			List.Insert(index, item);

			if (OnAdded != null)
				OnAdded(item);
		}

		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			var item = List[index];
			List.RemoveAt(index);

			if (OnRemoved != null)
				OnRemoved(item);
		}

		/// <inheritdoc />
		public bool Contains(T item)
		{
			return List.Contains(item);
		}

		/// <summary>
		///     Move an item to a new position.
		/// </summary>
		public void Move(int index, int newIndex)
		{
			if (index == newIndex)
				return;

			var item = List[index];
			List.RemoveAt(index);
			List.Insert(newIndex, item);

			if (OnMoved != null)
				OnMoved(item);
		}

		/// <inheritdoc />
		public void Clear()
		{
			while (List.Count > 0)
				RemoveAt(List.Count - 1);
		}

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)List).GetEnumerator();
		}
	}
}
