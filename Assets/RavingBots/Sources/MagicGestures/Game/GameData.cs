using RavingBots.MagicGestures.AI.Neural.Classic;

using UnityEngine;

namespace RavingBots.MagicGestures.Game
{
	/// <summary>
	///     The <c>ScriptableObject</c> holding the saved spell data and the trained network.
	/// </summary>
	/// <remarks>
	///     An instance of this object is saved as an asset so that spells can be
	///     pretrained and then shipped with the game. The current implementation is only
	///     capable of saving spells trained inside of an editor session.
	/// </remarks>
	/// <seealso cref="MagicGame.SaveOnQuit" />
	/// <seealso cref="SpellData" />
	/// <seealso cref="MultilayerPerceptron" />
	public class GameData : ScriptableObject
	{
		/// <summary>
		///     The trained neural network instance.
		/// </summary>
		public MultilayerPerceptron[] MLP = new MultilayerPerceptron[2];

		/// <summary>
		///     The configured spells.
		/// </summary>
		public SpellData[] Spells = new SpellData[0];

		/// <summary>
		///     Reset the saved data.
		/// </summary>
		public void Clear()
		{
			MLP = new MultilayerPerceptron[2];
			for (var i = 0; i < MLP.Length; i++)
			{
				MLP[i] = new MultilayerPerceptron();
			}
			Spells = new SpellData[0];

			Save();
		}

		/// <summary>
		///     Mark this asset as dirty and trigger a save.
		/// </summary>
		public void Save()
		{
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			UnityEditor.AssetDatabase.SaveAssets();

			Debug.LogFormat("{0} saved", name);
#else
			Debug.LogFormat("{0} not saved - saving currently supported in editor mode only", name);
#endif
		}
	}
}
