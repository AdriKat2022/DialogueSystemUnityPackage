using AdriKat.DialogueSystem.Conditions;
using UnityEditor;
using UnityEngine;

namespace AdriKat.DialogueSystem.Inspector
{
    public class CustomGameObjectContextMenu : MonoBehaviour
    {
        [MenuItem("GameObject/Dialogue System/Variable Condition Initializer", false, 10)]
        private static void CreateVarCondInitializer()
        {
            var gameObject = new GameObject("Variable Condition Initializer");
            gameObject.AddComponent<ConditionInitializer>();

            // Parent it to the current selected GameObject
            if (Selection.activeGameObject != null)
            {
                gameObject.transform.SetParent(Selection.activeGameObject.transform);
            }

            Selection.activeGameObject = gameObject;
        }
    }
}
