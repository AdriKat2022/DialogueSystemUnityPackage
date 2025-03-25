using System;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [Serializable]
    public class DialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DialogueSO NextDialogue { get; set; }

    }
}