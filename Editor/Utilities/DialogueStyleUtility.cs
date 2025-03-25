using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace AdriKat.DialogueSystem.Utility
{
    public static class DialogueStyleUtility
    {
        public static VisualElement AddStyleSheetsGUIDs(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                string path = AssetDatabase.GUIDToAssetPath(styleSheetName);
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                if (styleSheet == null)
                {
                    Debug.LogError($"Failed to load style sheet: {styleSheetName}");
                    continue;
                }
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetName);
                if (styleSheet == null)
                {
                    Debug.LogError($"Failed to load style sheet: {styleSheetName}");
                    continue;
                }
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }

        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }
    }
}