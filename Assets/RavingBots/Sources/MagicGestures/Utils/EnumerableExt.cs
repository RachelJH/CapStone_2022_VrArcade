using System;
using System.Collections;

namespace RavingBots.MagicGestures.Utils
{
	/// <summary>
	///     The extension methods for <see cref="IList" />.
	/// </summary>
	public static class EnumerableExt
	{
		/// <summary>
		///     Shuffle the given list.
		/// </summary>
		public static void Shuffle(this IList list, Random random)
		{
			for (var i = 0; i < list.Count - 1; i++)
			{
				var j = random.Next(i, list.Count);
				var tmp = list[i];
				list[i] = list[j];
				list[j] = tmp;
			}
		}
	}
}
