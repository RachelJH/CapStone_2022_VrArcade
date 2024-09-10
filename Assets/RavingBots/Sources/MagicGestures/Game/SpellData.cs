using System;
using System.Collections.Generic;
using System.Linq;
using RavingBots.MagicGestures.Controller;
using UnityEngine;

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace RavingBots.MagicGestures.Game
{
	/// <summary>
	///     A single configured spell.
	/// </summary>
	[Serializable]
	public class SpellData
	{
		/// <summary>
		///     The gestures input by the user when creating the spell.
		/// </summary>
		public GestureData[] Gestures;

		/// <summary>
		///     The color of the spell effect.
		/// </summary>
		public Color Color = Color.black;

		/// <summary>
		///     The index of the spell effect.
		/// </summary>
		/// <seealso cref="MagicGame.MagicEffectPrefabs" />
		public int EffectId = -1;

		/// <summary>
		///     <see langword="true" /> if the spell data is correct.
		/// </summary>
		/// <remarks>
		///     Must have at least one gesture, and all of the gestures must
		///     be valid.
		/// </remarks>
		/// <seealso cref="GestureData.IsValid" />
		public bool IsValid
		{
			get { return (Gestures.Length > 0) && !Gestures.Any(g => (g == null) || !g.IsValid); }
		}

		/// <summary>
		///     Construct a new spell with the given number of empty gestures.
		/// </summary>
		public SpellData(int size = 0)
		{
			Gestures = new GestureData[size];
		}

		/// <summary>
		///     Construct a new spell that's identical to an existing one.
		/// </summary>
		public SpellData(SpellData other)
		{
			Gestures = (GestureData[]) other.Gestures.Clone();
			Color = other.Color;
			EffectId = other.EffectId;
		}

		/// <summary>
		///     Construct a new spell based on an existing one, but with
		///     different gestures.
		/// </summary>
		public SpellData(SpellData other, IEnumerable<GestureData> gestures)
		{
			Gestures = gestures.ToArray();
			Color = other.Color;
			EffectId = other.EffectId;
		}

		/// <summary>
		///     Return the longest gesture in this spell.
		/// </summary>
		/// <seealso cref="GestureData.Length" />
		public GestureData FindLongest()
		{
			GestureData result = null;
			foreach (var g in Gestures)
				if ((result == null) || (result.Length[0] < g.Length[0]))
					result = g;

			return result;
		}

		/// <summary>
		///     Create the averaged preview gesture.
		/// </summary>
		public GestureData CreatePreview(float pointDensity)
		{
			var resampleSize = Mathf.RoundToInt(FindLongest().Length[0] * pointDensity);

			return GestureData.GetAveraged(Gestures, resampleSize);
		}
	}
}
