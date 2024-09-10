using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VRBuilder.Editor.StatesAndData.DemoScene
{
    /// <summary>
    /// Menu item for loading the demo scene after checking the process file is in the StreamingAssets folder.
    /// </summary>
    public static class DemoSceneLoader
    {
        private const string demoScenePath = "Assets/MindPort/VR Builder/Add-ons/StatesAndData/Demo/Scenes/VR Builder Demo - States and Data.unity";
        private const string demoProcessOrigin = "Assets/MindPort/VR Builder/Add-ons/StatesAndData/Demo/StreamingAssets/Processes/Demo - States and Data/Demo - States and Data.json";
        private const string demoProcessDirectory = "Assets/StreamingAssets/Processes/Demo - States and Data";
        private const string demoProcessDestination = "Assets/StreamingAssets/Processes/Demo - States and Data/Demo - States and Data.json";

        [MenuItem("Tools/VR Builder/Demo Scenes/States and Data", false, 64)]
        public static void LoadDemoScene()
        {
#if !VR_BUILDER_XR_INTERACTION
            if (EditorUtility.DisplayDialog("XR Interaction Component Required", "This demo scene requires VR Builder's built-in XR Interaction Component to be enabled. It looks like it is currently disabled. You can enable it in Project Settings > VR Builder > Settings.", "Ok")) 
            {
                return;
            }
#endif
            if (File.Exists(demoProcessDestination) == false)
            {
                if(EditorUtility.DisplayDialog("Demo Scene Setup", "Before opening the demo scene, the sample process needs to be copied in Assets/StreamingAssets. Press Ok to proceed.", "Ok"))
                {
                    Directory.CreateDirectory(demoProcessDirectory);
                    FileUtil.CopyFileOrDirectory(demoProcessOrigin, demoProcessDestination);
                }
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(demoScenePath);
        }
    }
}