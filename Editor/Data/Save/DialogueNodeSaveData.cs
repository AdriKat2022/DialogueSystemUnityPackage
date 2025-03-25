using AdriKat.DialogueSystem.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    // Not that great, the data needed by the exectuable dialogues and the conditional branches are untangled
    [System.Serializable]
    public class DialogueNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<DialogueChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public DialogueType Type { get; set; }
        [field: SerializeField] public ConditionType ConditionsToBeMet { get; set; }
        [field: SerializeField] public DialogueVariableNamesSO DialogueVariableNames { get; set; }
        [field: SerializeField] public List<DialogueConditionData> Conditions { get; set; }
        [field: SerializeField] public string NodeIDOnTrue { get; set; }
        [field: SerializeField] public string NodeIDOnFalse { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}