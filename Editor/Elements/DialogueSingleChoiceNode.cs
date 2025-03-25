using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using AdriKat.DialogueSystem.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AdriKat.DialogueSystem.Elements
{
    public class DialogueSingleChoiceNode : DialogueNode
    {
        public override void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);

            Type = DialogueType.SingleChoice;

            DialogueChoiceSaveData choiceData = new()
            {
                Text = "Next Dialogue",
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();


            foreach (DialogueChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);
                choicePort.userData = choice;
                //Debug.Log($"Drawing! ({choicePort.userData})");
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

    }
}