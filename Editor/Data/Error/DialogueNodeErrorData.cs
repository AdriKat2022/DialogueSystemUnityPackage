using AdriKat.DialogueSystem.Elements;
using System.Collections.Generic;

namespace AdriKat.DialogueSystem.Graph
{
    public class DialogueNodeErrorData
    {
        public DialogueErrorData ErrorData { get; set; }
        public List<DialogueNode> Nodes { get; set; }

        public DialogueNodeErrorData()
        {
            ErrorData = new DialogueErrorData();
            Nodes = new List<DialogueNode>();
        }
    }
}