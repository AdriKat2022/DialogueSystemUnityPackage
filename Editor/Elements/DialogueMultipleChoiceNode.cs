using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using AdriKat.DialogueSystem.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdriKat.DialogueSystem.Elements
{
    public class DialogueMultipleChoiceNode : DialogueNode
    {
        public override void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);
            Type = DialogueType.MultipleChoice;

            DialogueChoiceSaveData choiceData = new()
            {
                Text = "New Choice",
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            // Main Container
            Button addChoiceButton = DialogueElementUtility.CreateButton("Add Choice", () =>
            {
                DialogueChoiceSaveData choiceData = new()
                {
                    Text = "New Choice",
                };
                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);
                outputContainer.Add(choicePort);
            });
            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);

            // Output Container
            foreach (DialogueChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();
            choicePort.userData = userData;
            DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)userData;

            Button deleteButton = DialogueElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);
                graphView.RemoveElement(choicePort);
            });

            deleteButton.AddToClassList("ds-node__button");

            TextField choiceTextField = DialogueElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__choice-textfield",
                "ds-node__textfield__hidden"
                );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteButton);

            outputContainer.Add(choicePort);

            return choicePort;
        }
    }
}