using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    public class DialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }

        public void Initialize(bool isStartingDialogue)
        {
            IsStartingDialogue = isStartingDialogue;
        }
    }
}