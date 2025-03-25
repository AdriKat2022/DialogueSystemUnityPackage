using AdriKat.DialogueSystem.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [System.Serializable]
    public class ExecutableDialogueSO : DialogueSO
    {
        [field: SerializeField, TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DialogueType Type { get; set; }

        public void Initialize(string dialogueName, string text, List<DialogueChoiceData> choices, DialogueType dialogueType, bool isStartingDialogue)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            Type = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }
}