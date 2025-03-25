using AdriKat.DialogueSystem.Variables;
using AdriKat.Toolkit.Attributes;
using UnityEngine;

namespace AdriKat.DialogueSystem.Conditions
{
    public class ConditionInitializer : MonoBehaviour
    {
        [SerializeField] private bool _overrideAnimation;
        [ShowIf(nameof(_overrideAnimation)), Range(0, 5f)]
        [SerializeField] private float _animationTimer;
        [ShowIf(nameof(_overrideAnimation)), Range(0, 5f)]
        [SerializeField] private float _animationSmoothness;
        [ShowIf(nameof(_overrideAnimation), true, invert: true), Required("To initialize the variables, this needs a dialogue variable names container scriptable object.", RequiredAttribute.WarningTypeEnum.Error)]
        [SerializeField] private DialogueVariableNamesSO _dialogueVariablesNamesSO;
        [ShowIf(nameof(_overrideAnimation), true, invert: true), Required("Test field (if empty, shows warning of type error).", RequiredAttribute.WarningTypeEnum.Error)]
        [SerializeField] private DialogueVariableNamesSO _dialogueVariablesNamesS;
        [ShowIf(nameof(_overrideAnimation), true, invert: true), Required("Test field (if empty, shows warning of type warning).", RequiredAttribute.WarningTypeEnum.Error)]
        [SerializeField] private DialogueVariableNamesSO _dialogueVariablesNames;
        [ShowIf(nameof(_overrideAnimation), true, invert: true), Required("Test field (if empty, shows warning of type info).", RequiredAttribute.WarningTypeEnum.Info)]
        [SerializeField] private DialogueVariableNamesSO _dialogueVariablesName;

        [Enum("Test")]
        [SerializeField] private string test;
        [Enum("Test", "Test1")]
        [SerializeField] private string test1;
        [Enum(1, 2, 3, 4, 5, 6, 7)]
        [SerializeField] private int test2;
        [Enum("Use Controller", "Use Animator")]
        [SerializeField] private bool test3;
        [ShowIf(nameof(test3))]
        [SerializeField] private string animatorKeys;

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
        }
    }
}