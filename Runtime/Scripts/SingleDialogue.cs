using AdriKat.DialogueSystem.Core;
using UnityEngine;

namespace AdriKat.DialogueSystem.Triggers
{
    public class SingleDialogue : TriggerableDialogue
    {
        [SerializeField] private Dialogue _dialogue;
        [SerializeField] private DialogueController _dialogueBox;

        public override void Trigger()
        {
            if (_dialogue != null)
            {
                _dialogueBox.StartDialogue(_dialogue);
            }
        }
    }
}