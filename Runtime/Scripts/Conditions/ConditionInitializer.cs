using AdriKat.DialogueSystem.Variables;
using UnityEngine;

namespace AdriKat.DialogueSystem.Conditions
{
    public class ConditionInitializer : MonoBehaviour
    {
        [SerializeField] private bool _destroyAfterInstantiation = false;
        [SerializeField] private DialogueVariableNamesSO _dialogueVariablesNamesSO;

        private void Awake()
        {
            if (_dialogueVariablesNamesSO == null)
            {
                Debug.LogError("DialogueVariablesNamesSO not assigned in the ConditionInitializer script.");
            }
            else
            {
                DialogueVariables.SetDialogueVariablesNamesSO(_dialogueVariablesNamesSO);
            }

            if (_destroyAfterInstantiation)
            {
                Destroy(gameObject);
            }
        }
    }
}