using AdriKat.DialogueSystem.Conditions;
using AdriKat.DialogueSystem.Core;
using UnityEditor;
using UnityEngine;

namespace AdriKat.DialogueSystem.Inspector
{
    public class CustomGameObjectContextMenu : MonoBehaviour
    {
        [MenuItem("GameObject/Dialogue System/Condition Variable Initializer", false, 10)]
        private static void CreateVarCondInitializer()
        {
            var gameObject = new GameObject("Condition Variable Initializer");
            gameObject.AddComponent<ConditionInitializer>();

            // Parent it to the current selected GameObject
            if (Selection.activeGameObject != null)
            {
                gameObject.transform.SetParent(Selection.activeGameObject.transform);
            }

            Selection.activeGameObject = gameObject;
        }


        [MenuItem("GameObject/Dialogue System/Condition Variable Modifier", false, 10)]
        private static void CreateVarCondModifier()
        {
            var gameObject = new GameObject("Condition Variable Modifier");
            gameObject.AddComponent<ConditionVariableModifier>();

            // Parent it to the current selected GameObject
            if (Selection.activeGameObject != null)
            {
                gameObject.transform.SetParent(Selection.activeGameObject.transform);
            }

            Selection.activeGameObject = gameObject;
        }


        [MenuItem("GameObject/Dialogue System/Dialogue Selector", false, 10)]
        private static void CreateDialogueSelector()
        {
            var gameObject = new GameObject("Dialogue");
            gameObject.AddComponent<Dialogue>();

            // Parent it to the current selected GameObject
            if (Selection.activeGameObject != null)
            {
                gameObject.transform.SetParent(Selection.activeGameObject.transform);
            }

            Selection.activeGameObject = gameObject;
        }
    }
}
