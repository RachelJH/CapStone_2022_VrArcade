/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for adding SSS tool menu, menu context, and gameobejct items.
*******************************************************************************************/
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace StickyStickStuck
{
    public class SSS_Tool : EditorWindow
    {
        //Constructor
        public SSS_Tool()
        {
        }

        #region MenuItems / ToolBar

        [MenuItem("Tools/Resurgam Studios/SSS/Add SSS Component", false, 2)]
        public static void AddSSSComponent()
        {
            Selection.activeGameObject.AddComponent<SSS>();
        }

        [MenuItem("Tools/Resurgam Studios/SSS/Add SSS 2D Component", false, 3)]
        public static void AddSSS2DComponent()
        {
            Selection.activeGameObject.AddComponent<SSS2D>();
        }

        [MenuItem("Tools/Resurgam Studios/SSS/Clear All SSS From Scene", false, 4)]
        public static void ClearAllSSSFromScene()
        {
            var sssList = GameObject.FindObjectsOfType<SSS>();
            var sssList2D = GameObject.FindObjectsOfType<SSS2D>();

            int count = sssList.Length + sssList2D.Length;

            if (count > 0)
            {
                string message = string.Format("You are about to remove {0} SSS components from the scene.", count.ToString());

                if (EditorUtility.DisplayDialog("Warning", message, "Ok", "cancel"))
                {
                    for (int i = 0; i < sssList.Length; i++)
                    {
                        DestroyImmediate(sssList[i]);
                    }
                    for (int i = 0; i < sssList2D.Length; i++)
                    {
                        DestroyImmediate(sssList2D[i]);
                    }
                }
            }
            else
            {
                string message = string.Format("No SSS components found in scene.", count.ToString());

                EditorUtility.DisplayDialog("Warning", message, "Ok");
            }
        }

        [MenuItem("Tools/Resurgam Studios/SSS/Add SSS Component", true)]
        [MenuItem("Tools/Resurgam Studios/SSS/Add SSS 2D Component", true)]
        public static bool SSS_Validation()
        { 
            if (Selection.activeGameObject == null)
            {
                return false;
            }

            return true;
        }

        [MenuItem("Tools/Resurgam Studios/SSS/Support/Discord", false)]
        public static void SupportDiscord()
        {
            Application.OpenURL("https://discord.gg/KGZyncGDrb");
        }
        [MenuItem("Tools/Resurgam Studios/SSS/Support/Unity Form", false)]
        public static void SupportUnityForm()
        {
            Application.OpenURL("http://forum.unity3d.com/threads/sticky-stick-stuck.228315/");
        }
        [MenuItem("Tools/Resurgam Studios/SSS/Support/Asset Store", false)]
        public static void SupportAssetStore()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/15021");
        }
        [MenuItem("Tools/Resurgam Studios/SSS/Support/Website", false)]
        public static void SupportWebsite()
        {
            Application.OpenURL("https://www.resurgamstudios.com/sss");
        }
        [MenuItem("Tools/Resurgam Studios/SSS/Support/Version History", false)]
        public static void SupportVersionHistory()
        {
            Application.OpenURL("https://www.resurgamstudios.com/sss-version-history");
        }

        #endregion
    }
}