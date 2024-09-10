/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
*******************************************************************************************/
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

namespace CircularGravityForce
{
    public static class InstalledPackage
    {
        private static string MessageID = "ResurgamStudios";
        private static string DiscordLink = "https://discord.gg/KGZyncGDrb";

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            int readMessage = PlayerPrefs.GetInt(MessageID);

            if (readMessage == 0)
            {
                bool message = EditorUtility.DisplayDialog("Resurgam Studios Content",
                    "Thanks for purchasing Circular Gravity Force! Be sure to check out our support options along with our community Discord channel for questions, requests, issues or to hangout!\n\nTools\\Resurgam-Studios\\CGF\\Support\n",
                    "Join Discord",
                    "Done");

                if (message)
                    Application.OpenURL(DiscordLink);

                PlayerPrefs.SetInt(MessageID, 1);
            }
        }
    }
}