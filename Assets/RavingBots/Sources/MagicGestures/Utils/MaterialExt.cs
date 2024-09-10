using UnityEngine;

namespace RavingBots.MagicGestures.Utils
{
	/// <summary>
	///     The extension methods for <c>Material</c>.
	/// </summary>
	public static class MaterialExt
	{
		/// <summary>
		///     Set the emission color of this material.
		/// </summary>
		public static void SetEmission(this Material m, Color color)
		{
			m.SetColor("_EmissionColor", color);
		}

		/// <summary>
		///     Return the emission color of this material.
		/// </summary>
		public static Color GetEmission(this Material m)
		{
			return m.GetColor("_EmissionColor");
		}
	}
}
