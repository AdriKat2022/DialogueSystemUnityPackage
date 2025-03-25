using AdriKat.DialogueSystem.Conditions;

namespace AdriKat.DialogueSystem.Inspector.Conditions
{
    using AdriKat.DialogueSystem.Variables;
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ConditionVariableModifier))]
    public class ConditionModifierInspector : Editor
    {
        // Get all the properties of the Condition script
        private SerializedProperty _dialogueVariablesNamesSO;

        private SerializedProperty _modifierValueType;

        private SerializedProperty _boolKey;
        private SerializedProperty _boolModifierType;
        private SerializedProperty _boolValue;

        private SerializedProperty _intKey;
        private SerializedProperty _intModifierType;
        private SerializedProperty _intValue;

        private SerializedProperty _stringKey;
        private SerializedProperty _stringModifierType;
        private SerializedProperty _stringValue;

        private void OnEnable()
        {
            _dialogueVariablesNamesSO = serializedObject.FindProperty("_dialogueVariablesNamesSO");

            _modifierValueType = serializedObject.FindProperty("_modifierValueType");

            _boolKey = serializedObject.FindProperty("_boolKey");
            _boolModifierType = serializedObject.FindProperty("_boolModifierType");
            _boolValue = serializedObject.FindProperty("_boolValue");

            _intKey = serializedObject.FindProperty("_intKey");
            _intModifierType = serializedObject.FindProperty("_intModifierType");
            _intValue = serializedObject.FindProperty("_intValue");

            _stringKey = serializedObject.FindProperty("_stringKey");
            _stringModifierType = serializedObject.FindProperty("_stringModifierType");
            _stringValue = serializedObject.FindProperty("_stringValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Dialogue Variables Names", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_dialogueVariablesNamesSO);

            if (_dialogueVariablesNamesSO == null || _dialogueVariablesNamesSO.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign the DialogueVariableNamesSO scriptable object to get the variable names." +
                    "To create one, Right Click -> Dialogue -> Condition Names.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Condition Type", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_modifierValueType);

            EditorGUILayout.Space(16);

            switch ((DialogueVariableType)_modifierValueType.enumValueIndex)
            {
                case DialogueVariableType.Bool:
                    DrawDropdownForPropertyAndType(_boolKey, DialogueVariableType.Bool);
                    EditorGUILayout.PropertyField(_boolModifierType);
                    EditorGUILayout.PropertyField(_boolValue);
                    break;
                case DialogueVariableType.Int:
                    // Display the three properties aside each
                    DrawDropdownForPropertyAndType(_intKey, DialogueVariableType.Int);
                    EditorGUILayout.PropertyField(_intModifierType);
                    EditorGUILayout.PropertyField(_intValue);
                    break;
                case DialogueVariableType.String:
                    DrawDropdownForPropertyAndType(_stringKey, DialogueVariableType.String);
                    EditorGUILayout.PropertyField(_stringModifierType);
                    EditorGUILayout.PropertyField(_stringValue);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDropdownForPropertyAndType(SerializedProperty property, DialogueVariableType type)
        {
            // Get the names of the variables based on the type
            DialogueVariableNamesSO dialogueVariableNames = _dialogueVariablesNamesSO.objectReferenceValue as DialogueVariableNamesSO;
            string[] variableNames = dialogueVariableNames.GetVarNames(type);

            // If the variable names are not found, show a warning
            if (variableNames == null || variableNames.Length == 0)
            {
                EditorGUILayout.HelpBox($"No names were found for this type! Create one in the {dialogueVariableNames.name} object.", MessageType.Warning);
                return;
            }

            int selected = Array.IndexOf(variableNames, property.stringValue);
            selected = EditorGUILayout.Popup("Variable Name", selected, variableNames);
            selected = Mathf.Clamp(selected, 0, variableNames.Length - 1);

            property.stringValue = variableNames[selected];
        }
    }
}