/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used overwriting the Inspector GUI, and Scene GUI.
*******************************************************************************************/
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace StickyStickStuck
{
    [Serializable]
    public class TitleImage
    {
        public Texture2D image;
        public GUIContent content;

        public TitleImage(string imagePath)
        {
            content = new GUIContent("");
            this.image = (Texture2D)AssetDatabase.LoadAssetAtPath(imagePath, typeof(Texture2D));
            content.image = this.image;
            
        }
    }

    [CustomEditor(typeof(SSS)), CanEditMultipleObjects]
    public class SSS_Editor : Editor
    {
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private TitleImage titleImage;

        private SerializedProperty enable_property;
        private SerializedProperty paused_property;
        private SerializedProperty parentType_property;
        private SerializedProperty jointType_property;
        private SerializedProperty stickToTrigger_property;
        private SerializedProperty stickInTriggerArea_property;
        private SerializedProperty stickImpactVelocity_property;
        private SerializedProperty stickToNonRigidbody_property;
        private SerializedProperty stickOnSticky_property;
        private SerializedProperty stickOffset_property;
        private SerializedProperty ignoreColliders_property;
        private SerializedProperty stickRecursively_property;
        private SerializedProperty maxEffectedItems_property;

        private string helpText;
        private MessageType messageType = MessageType.Info;

        private bool change = false;

        public void OnEnable()
        {
            //Title Image Resource
            titleImage = new TitleImage("Assets/ResurgamStudios/StickyStickStuck Package/Gizmos/SSS Title.png");

            enable_property = serializedObject.FindProperty("enable");
            paused_property = serializedObject.FindProperty("paused");
            parentType_property = serializedObject.FindProperty("parentType");
            jointType_property = serializedObject.FindProperty("jointType");
            stickToTrigger_property = serializedObject.FindProperty("stickToTrigger");
            stickInTriggerArea_property = serializedObject.FindProperty("stickInTriggerArea");
            stickImpactVelocity_property = serializedObject.FindProperty("stickImpactVelocity");
            stickToNonRigidbody_property = serializedObject.FindProperty("stickToNonRigidbody");
            stickOnSticky_property = serializedObject.FindProperty("stickOnSticky");
            stickOffset_property = serializedObject.FindProperty("stickOffset");
            ignoreColliders_property = serializedObject.FindProperty("ignoreColliders");
            stickRecursively_property = serializedObject.FindProperty("stickRecursively");
            maxEffectedItems_property = serializedObject.FindProperty("maxEffectedItems");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            /*<----------------------------------------------------------------------------------------------------------*/
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.padding = new RectOffset(panelStyle.padding.left + 10, panelStyle.padding.right, panelStyle.padding.top, panelStyle.padding.bottom);
            EditorGUILayout.BeginVertical(panelStyle);
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.UpperCenter;
            GUILayout.BeginVertical();
            GUILayout.Box(titleImage.content, titleStyle);
            GUILayout.EndVertical();
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(enable_property, new GUIContent("Enable"));
            if (EditorGUI.EndChangeCheck())
            {
                enable_property.boolValue = EditorGUILayout.Toggle(enable_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(paused_property, new GUIContent("Paused"));
            if (EditorGUI.EndChangeCheck())
            {
                paused_property.boolValue = EditorGUILayout.Toggle(paused_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(parentType_property, new GUIContent("Parent Type"));
            if (EditorGUI.EndChangeCheck())
            {
                parentType_property.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup((SSS.ParentType)parentType_property.enumValueIndex));
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(jointType_property, new GUIContent("Joint Type"));
            if (EditorGUI.EndChangeCheck())
            {
                jointType_property.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup((SSS.JointType)jointType_property.enumValueIndex));
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            if (helpText != string.Empty)
                EditorGUILayout.HelpBox(helpText, messageType, true);

            switch (jointType_property.enumValueIndex)
            {
                case (int)SSS.JointType.Fixed:

                    helpText = "Uses Unity's Fixed Joint to restricts an object’s movement to be dependent upon another object that it sticks to.";
                    messageType = MessageType.Info;

                    /*<----------------------------------------------------------------------------------------------------------*/
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fixedJointProperties"), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    /*<----------------------------------------------------------------------------------------------------------*/

                    break;
                case (int)SSS.JointType.Character:

                    helpText = "Uses Unity's Character Joint to sick objects together like a ragdoll effect.";
                    messageType = MessageType.Info;

                    /*<----------------------------------------------------------------------------------------------------------*/
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("characterJointProperties"), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    /*<----------------------------------------------------------------------------------------------------------*/

                    break;
                case (int)SSS.JointType.Spring:

                    helpText = "Uses Unity's Spring Joint to joint two Rigidbodies together but allows the distance between them to change as though they were connected by a spring.";
                    messageType = MessageType.Info;

                    /*<----------------------------------------------------------------------------------------------------------*/
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("springJointProperties"), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    /*<----------------------------------------------------------------------------------------------------------*/

                    break;
                case (int)SSS.JointType.Hinge:

#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3)
                        helpText = SSS.WarningMessageHingeUnity_5_4;
                        messageType = MessageType.Warning;
#else
                    helpText = "Uses Unity's Hinge Joint to group together two Rigidbodies, constraining them to move like they are connected by a hinge.";
                        messageType = MessageType.Info;
                    #endif
                    
                    /*<----------------------------------------------------------------------------------------------------------*/
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hingeJointProperties"), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    /*<----------------------------------------------------------------------------------------------------------*/

                    break;
                case (int)SSS.JointType.Configurable:
                    helpText = "Uses Unity's Configurable Joints incorporate all the functionality of the other joint types and provide greater control of character movement.";
                    messageType = MessageType.Info;

                    /*<----------------------------------------------------------------------------------------------------------*/
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("configurableJointProperties"), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    /*<----------------------------------------------------------------------------------------------------------*/
                    break;
                case (int)SSS.JointType.None:
                default:
                    helpText = "";
                    break;
            }

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventProperties"), true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterProperties"), true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stickToTrigger_property, new GUIContent("Stick To Trigger"));
            if (EditorGUI.EndChangeCheck())
            {
                stickToTrigger_property.boolValue = EditorGUILayout.Toggle(stickToTrigger_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stickInTriggerArea_property, new GUIContent("Stick In Trigger Area"));
            if (EditorGUI.EndChangeCheck())
            {
                stickInTriggerArea_property.boolValue = EditorGUILayout.Toggle(stickInTriggerArea_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stickToNonRigidbody_property, new GUIContent("Stick To Non Rigidbody"));
            if (EditorGUI.EndChangeCheck())
            {
                stickToNonRigidbody_property.boolValue = EditorGUILayout.Toggle(stickToNonRigidbody_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stickOnSticky_property, new GUIContent("Stick On Sticky"));
            if (EditorGUI.EndChangeCheck())
            {
                stickOnSticky_property.boolValue = EditorGUILayout.Toggle(stickOnSticky_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stickOffset_property, new GUIContent("Stick Offset"));
            if (EditorGUI.EndChangeCheck())
            {
                stickOffset_property.vector3Value = EditorGUILayout.Vector3Field("Stick Offset", stickOffset_property.vector3Value);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stickImpactVelocity_property, new GUIContent("Stick Impact Velocity"));
            if (EditorGUI.EndChangeCheck())
            {
                stickImpactVelocity_property.floatValue = EditorGUILayout.FloatField(stickImpactVelocity_property.floatValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(ignoreColliders_property, new GUIContent("Ignore Colliders"));
            if (EditorGUI.EndChangeCheck())
            {
                ignoreColliders_property.boolValue = EditorGUILayout.Toggle(ignoreColliders_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stickRecursively_property, new GUIContent("Stick Recursively"));
            if (EditorGUI.EndChangeCheck())
            {
                stickRecursively_property.boolValue = EditorGUILayout.Toggle(stickRecursively_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(maxEffectedItems_property, new GUIContent("Max Effected Items"));
            if (EditorGUI.EndChangeCheck())
            {
                maxEffectedItems_property.intValue = EditorGUILayout.IntField(maxEffectedItems_property.intValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("debugProperties"), true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            /*<----------------------------------------------------------------------------------------------------------*/

			EditorGUILayout.EndVertical();
			
            if (change)
            {
                change = false;
            }

            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
