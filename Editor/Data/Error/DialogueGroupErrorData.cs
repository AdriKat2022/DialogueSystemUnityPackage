using AdriKat.DialogueSystem.Elements;
using System.Collections.Generic;

namespace AdriKat.DialogueSystem.Graph
{
    public class DialogueGroupErrorData
    {
        public DialogueErrorData ErrorData { get; set; }
        public List<DialogueGroup> Groups { get; set; }

        public DialogueGroupErrorData()
        {
            ErrorData = new DialogueErrorData();
            Groups = new List<DialogueGroup>();
        }
    }
}