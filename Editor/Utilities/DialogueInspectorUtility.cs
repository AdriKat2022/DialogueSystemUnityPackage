using System;
using UnityEditor;

namespace AdriKat.DialogueSystem.Utility
{
    public static class DialogueInspectorUtility
    {
        public static void DrawDisabledFields(Action action)
        {
            EditorGUI.BeginDisabledGroup(true);
            action.Invoke();
            EditorGUI.EndDisabledGroup();
        }

        public static void DrawHeader(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        public static void DrawPropertyField(this SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property);
        }

        public static void DrawPopup(this SerializedProperty property, string label, string[] options)
        {
            property.intValue = EditorGUILayout.Popup(label, property.intValue, options);
        }

        public static int DrawPopup(string label, int selectedIndex, string[] options)
        {
            return EditorGUILayout.Popup(label, selectedIndex, options);
        }

        public static void DrawSpace(float pixels = 4)
        {
            EditorGUILayout.Space(pixels);
        }
    }
}