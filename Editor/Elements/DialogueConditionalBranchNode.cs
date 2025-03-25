using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using AdriKat.DialogueSystem.Utility;
using AdriKat.DialogueSystem.Variables;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdriKat.DialogueSystem.Elements
{
    public class DialogueConditionalBranchNode : DialogueNode
    {
        [field: SerializeField] public DialogueVariableNamesSO DialogueVariableNames { get; set; }
        [field: SerializeField] public List<DialogueConditionData> Conditions { get; set; }
        [field: SerializeField] public ConditionType ConditionToBeMet { get; set; }
        [field: SerializeField] public string NodeOnTrue { get; set; }
        [field: SerializeField] public string NodeOnFalse { get; set; }

        public override void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);
            Conditions = new List<DialogueConditionData>
            {
                new DialogueConditionData()
            };
            Type = DialogueType.ConditionalBranch;
            ConditionToBeMet = ConditionType.All;
            NodeOnTrue = string.Empty;
            NodeOnFalse = string.Empty;
            showDialogueTextContents = false;
        }

        public override void Draw()
        {
            base.Draw();

            // VariableNamesSO Selection Field
            ObjectField variableNamesField = new ObjectField("Variable Names SO")
            {
                objectType = typeof(DialogueVariableNamesSO),
                allowSceneObjects = false,
                value = DialogueVariableNames
            };

            variableNamesField.RegisterValueChangedCallback(evt =>
            {
                DialogueVariableNames = evt.newValue as DialogueVariableNamesSO;
            });

            // Button to Auto-Select the First Available VariableNamesSO
            Button selectFirstButton = new Button(() =>
            {
                DialogueVariableNamesSO firstAvailable = FindFirstVariableNamesSO();
                if (firstAvailable != null)
                {
                    DialogueVariableNames = firstAvailable;
                    variableNamesField.value = firstAvailable;
                }
            })
            {
                text = "Select First Available"
            };

            // Add elements to UI
            extensionContainer.Add(variableNamesField);
            extensionContainer.Add(selectFirstButton);

            // Enum Dropdown to choose the condition to be met
            EnumField conditionTypeField = new("Conditions To Be Met", ConditionToBeMet);
            conditionTypeField.RegisterValueChangedCallback(evt => ConditionToBeMet = (ConditionType)evt.newValue);
            extensionContainer.Add(conditionTypeField);

            // Conditions Container
            VisualElement customConditionsContainer = new VisualElement();
            customConditionsContainer.AddToClassList("ds-node__custom-data-container");
            extensionContainer.Add(customConditionsContainer);

            Foldout conditionsFoldout = DialogueElementUtility.CreateFoldout("Conditions");
            conditionsFoldout.AddToClassList("ds-node__foldout");
            customConditionsContainer.Insert(0, conditionsFoldout);

            // Main Container
            Button addConditionButton = DialogueElementUtility.CreateButton("Add Condition", () =>
            {
                DialogueConditionData conditionData = new();

                CreateCondition(conditionData, conditionsFoldout);
            });
            addConditionButton.AddToClassList("ds-node__button");
            customConditionsContainer.Insert(0, addConditionButton);

            for (int i = 0; i < Conditions.Count; i++)
            {
                DrawCondition(Conditions[i], conditionsFoldout);
            }

            // Output Container, make two ports for true and false
            Port truePort = this.CreatePort("True", Orientation.Horizontal, Direction.Output);
            truePort.userData = NodeOnTrue;
            outputContainer.Add(truePort);

            Port falsePort = this.CreatePort("False", Orientation.Horizontal, Direction.Output);
            falsePort.userData = NodeOnFalse;
            outputContainer.Add(falsePort);

            RefreshExpandedState();
        }

        private void CreateCondition(DialogueConditionData conditionalData, Foldout conditionsContainer)
        {
            DrawCondition(conditionalData, conditionsContainer);

            Conditions.Add(conditionalData);
        }

        private void DrawCondition(DialogueConditionData conditionalData, Foldout conditionsContainer)
        {
            VisualElement conditionContainer = new();
            conditionContainer.AddToClassList("condition-container");

            // Condition Type Dropdown
            EnumField conditionTypeField = new EnumField("", conditionalData.ConditionValueType)
            {
                style = { flexGrow = 1 } // Makes it take up available space
            };
            conditionTypeField.labelElement.style.display = DisplayStyle.None; // Hides the label
            conditionTypeField.RegisterValueChangedCallback(evt =>
            {
                conditionalData.ConditionValueType = (Variables.DialogueVariableType)evt.newValue;
                RefreshConditionUI(conditionContainer, conditionalData);
            });

            // Variable Name Dropdown
            DropdownField variableDropdown = new("Variable Name")
            {
                choices = GetVariableNamesForType(conditionalData.ConditionValueType),
                value = conditionalData.Key
            };
            variableDropdown.RegisterValueChangedCallback(evt => conditionalData.Key = evt.newValue);

            // Comparison Type Dropdown (Dynamically Updated)
            VisualElement comparisonTypeContainer = new();
            EnumField comparisonTypeField = CreateComparisonTypeField(conditionalData);
            comparisonTypeContainer.Add(comparisonTypeField);

            // Value Input Field
            VisualElement valueField = CreateValueField(conditionalData);

            // Delete Button
            Button deleteButton = DialogueElementUtility.CreateButton("Remove Condition", () =>
            {
                if (Conditions.Count < 1) return;

                Conditions.Remove(conditionalData);
                conditionsContainer.Remove(conditionContainer);
            });
            deleteButton.AddToClassList("ds-node__button");

            // Add UI Elements to Container
            conditionContainer.Add(conditionTypeField);
            conditionContainer.Add(variableDropdown);
            conditionContainer.Add(comparisonTypeContainer);
            conditionContainer.Add(valueField);
            conditionContainer.Add(deleteButton);

            // Add the condition container to the conditions list
            conditionsContainer.Add(conditionContainer);

            // Refresh UI when type changes
            conditionTypeField.RegisterValueChangedCallback(evt =>
            {
                conditionalData.ConditionValueType = (Variables.DialogueVariableType)evt.newValue;
                comparisonTypeContainer.Clear();
                comparisonTypeContainer.Add(CreateComparisonTypeField(conditionalData));
            });
        }

        // Utility Method: Create Comparison Type Field
        private EnumField CreateComparisonTypeField(DialogueConditionData conditionalData)
        {
            EnumField comparisonTypeField;

            switch (conditionalData.ConditionValueType)
            {
                case Variables.DialogueVariableType.Bool:
                    comparisonTypeField = new EnumField("Comparison", conditionalData.BoolComparisonType);
                    comparisonTypeField.RegisterValueChangedCallback(evt =>
                    {
                        conditionalData.BoolComparisonType = (BoolComparisonType)evt.newValue;
                    });
                    break;

                case Variables.DialogueVariableType.Int:
                    comparisonTypeField = new EnumField("Comparison", conditionalData.IntComparisonType);
                    comparisonTypeField.RegisterValueChangedCallback(evt =>
                    {
                        conditionalData.IntComparisonType = (IntComparisonType)evt.newValue;
                    });
                    break;

                case Variables.DialogueVariableType.String:
                    comparisonTypeField = new EnumField("Comparison", conditionalData.StringComparisonType);
                    comparisonTypeField.RegisterValueChangedCallback(evt =>
                    {
                        conditionalData.StringComparisonType = (StringComparisonType)evt.newValue;
                    });
                    break;

                default:
                    return new EnumField("Comparison", null);
            }

            return comparisonTypeField;
        }

        // Utility Method: Refresh UI when changing condition type
        private void RefreshConditionUI(VisualElement container, DialogueConditionData conditionData)
        {
            container.Clear();
            DrawCondition(conditionData, (Foldout)container.parent);
        }

        // Utility Method: Create a Value Field Based on Condition Type
        private VisualElement CreateValueField(DialogueConditionData conditionalData)
        {
            switch (conditionalData.ConditionValueType)
            {
                case Variables.DialogueVariableType.Bool:
                    Toggle boolToggle = new("Value");
                    boolToggle.value = conditionalData.BoolValue;
                    boolToggle.RegisterValueChangedCallback(evt => conditionalData.BoolValue = evt.newValue);
                    return boolToggle;

                case Variables.DialogueVariableType.Int:
                    IntegerField intField = new("Value");
                    intField.value = conditionalData.IntValue;
                    intField.RegisterValueChangedCallback(evt => conditionalData.IntValue = evt.newValue);
                    return intField;

                case Variables.DialogueVariableType.String:
                    TextField stringField = new("Value");
                    stringField.value = conditionalData.StringValue;
                    stringField.RegisterValueChangedCallback(evt => conditionalData.StringValue = evt.newValue);
                    return stringField;

                default:
                    return new Label("Unsupported Type");
            }
        }

        // Utility Method: Get Variable Names for the Selected Type
        private List<string> GetVariableNamesForType(DialogueVariableType type)
        {
            // Fetch variable names from your DialogueVariableNamesSO
            if (DialogueVariableNames != null)
            {
                return new List<string>(DialogueVariableNames.GetVarNames(type));
            }

            return new List<string>() { "----No SO Selected----" };
        }

        // Method to Find the First Available VariableNamesSO in the Project
        private DialogueVariableNamesSO FindFirstVariableNamesSO()
        {
            string[] guids = AssetDatabase.FindAssets("t:DialogueVariableNamesSO");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<DialogueVariableNamesSO>(path);
            }
            return null;
        }
    }
}