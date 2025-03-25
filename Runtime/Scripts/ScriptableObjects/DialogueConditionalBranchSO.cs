using AdriKat.DialogueSystem.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    public class DialogueConditionalBranchSO : DialogueSO
    {
        [field: SerializeField] public DialogueVariableNamesSO DialogueVariableNames { get; set; }
        [field: SerializeField] public List<DialogueConditionData> Conditions { get; set; }
        [field: SerializeField] public ConditionType ConditionsToBeMet { get; set; }
        [field: SerializeField] public DialogueSO DialogueOnTrue { get; set; }
        [field: SerializeField] public DialogueSO DialogueOnFalse { get; set; }

        public void Initialize(List<DialogueConditionData> condition, ConditionType conditionsToBeMet, DialogueSO dialogueOnTrue, DialogueSO dialogueOnFalse, bool isStartingDialogue)
        {
            Conditions = condition;
            ConditionsToBeMet = conditionsToBeMet;
            DialogueOnTrue = dialogueOnTrue;
            DialogueOnFalse = dialogueOnFalse;
            IsStartingDialogue = isStartingDialogue;
        }
    }
}