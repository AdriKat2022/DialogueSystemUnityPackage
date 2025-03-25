using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using AdriKat.DialogueSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdriKat.DialogueSystem.Elements
{
    public class DialogueNode : Node
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] public string DialogueText { get; set; }
        [field: SerializeField] public List<DialogueChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public DialogueType Type { get; set; }
        [field: SerializeField] public DialogueGroup Group { get; set; }

        protected DialogueGraphView graphView;
        private Color defaultBackgroundColor;
        protected bool showDialogueTextContents = true;

        public virtual void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            this.graphView = graphView;
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            Choices = new List<DialogueChoiceSaveData>();
            DialogueText = "DialogueText";

            defaultBackgroundColor = new Color(29 / 255f, 29 / 255f, 30 / 255f);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        #region Overrided methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", action => { DisconnectInputPorts(); });
            evt.menu.AppendAction("Disconnect Output Ports", action => { DisconnectOutputPorts(); });

            base.BuildContextualMenu(evt);
        }
        #endregion

        public virtual void Draw()
        {
            // Title container
            TextField dialogueNameTextField = DialogueElementUtility.CreateTextField(DialogueName, null, evt =>
            {
                TextField target = (TextField)evt.target;
                target.value = evt.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                // Keep track of error inducing names
                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        graphView.NameErrorAmount++;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        graphView.NameErrorAmount--;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);
                    DialogueName = evt.newValue;
                    graphView.AddUngroupedNode(this);
                }
                else
                {
                    // Save the current group, cause it will be removed when the node is removed
                    DialogueGroup currentGroup = Group;
                    graphView.RemoveGroupedNode(this, Group);
                    DialogueName = evt.newValue;
                    graphView.AddGroupedNode(this, currentGroup);
                }
            });
            dialogueNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
                );
            titleContainer.Insert(0, dialogueNameTextField);

            // Input container
            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);


            // Extensions container
            if (showDialogueTextContents)
            {
                DrawDialogueText();
            }
        }

        private void DrawDialogueText()
        {
            VisualElement customDataContainer = new();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DialogueElementUtility.CreateFoldout("Dialogue Text");

            TextField textTextField = DialogueElementUtility.CreateTextArea(DialogueText, null, callback =>
            {
                DialogueText = callback.newValue;
            });

            textTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__quote-textfield"
                );

            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
        }

        #region Utility Methods

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        public void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        public void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }

        public bool IsStartingNode()
        {
            Port inputPort = inputContainer.Children().First() as Port;

            return !inputPort.connected;
        }
        #endregion
    }
}