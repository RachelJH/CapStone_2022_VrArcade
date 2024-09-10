using System.IO;
using UnityEditor;
using UnityEngine;

namespace RavingBots.MagicGestures.Editor.Utils
{
	public static class ScriptableObjectUtils
	{
		public static T CreateAsset<T>(string name) where T : ScriptableObject
		{
			var result = ScriptableObject.CreateInstance<T>();

			var path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path == "")
				path = "Assets";
			else if (Path.GetExtension(path) != "")
				path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

			var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

			AssetDatabase.CreateAsset(result, assetPathAndName);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = result;

			return result;
		}
	}
}
