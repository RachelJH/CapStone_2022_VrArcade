using RavingBots.MagicGestures.Editor.Utils;
using RavingBots.MagicGestures.Game;
using UnityEditor;
using UnityEngine;

namespace RavingBots.MagicGestures.Editor
{
	[CustomEditor(typeof(GameData))]
	[CanEditMultipleObjects]
	public class GameDataEditor : UnityEditor.Editor
	{
		[MenuItem("Assets/Create/Game Data", false, 32)]
		public static GameData CreateGameData()
		{
			return ScriptableObjectUtils.CreateAsset<GameData>("Game Data");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Clear"))
				foreach (GameData t in targets)
					t.Clear();
		}
	}
}
