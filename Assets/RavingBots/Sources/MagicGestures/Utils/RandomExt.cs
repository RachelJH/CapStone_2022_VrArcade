using System;

namespace RavingBots.MagicGestures.Utils
{
	/// <summary>
	///     The extension methods for <see cref="Random" />.
	/// </summary>
	public static class RandomExt
	{
		/// <summary>
		///     Return a new random double in the given range.
		/// </summary>
		public static double NextDouble(this Random random, double a, double b)
		{
			return random.NextDouble() * (b - a) + a;
		}
	}
}
