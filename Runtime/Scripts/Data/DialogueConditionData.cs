using AdriKat.DialogueSystem.Variables;
using System;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [Serializable]
    public class DialogueConditionData
    {
        [field: SerializeField] public DialogueVariableNamesSO DialogueVariablesNamesSO;

        [field: SerializeField] public DialogueVariableType ConditionValueType { get; set; }
        [field: Space]
        [field: SerializeField] public string Key { get; set; }

        [field: SerializeField] public BoolComparisonType BoolComparisonType { get; set; }
        [field: SerializeField] public bool BoolValue { get; set; }
        [field: Space]
        [field: SerializeField] public IntComparisonType IntComparisonType { get; set; }
        [field: SerializeField] public int IntValue { get; set; }
        [field: Space]
        [field: SerializeField] public StringComparisonType StringComparisonType { get; set; }
        [field: SerializeField] public string StringValue { get; set; }

        public bool Evaluate()
        {
            switch (ConditionValueType)
            {
                case DialogueVariableType.Bool:
                    bool? value = DialogueVariables.GetBool(Key);

                    if (value != null)
                    {
                        // Swtich return according to the bool comparison
                        return BoolComparisonType switch
                        {
                            BoolComparisonType.Is => (bool)value == BoolValue,
                            BoolComparisonType.And => (bool)value && BoolValue,
                            BoolComparisonType.Or => (bool)value || BoolValue,
                            BoolComparisonType.Xor => (bool)value ^ BoolValue,
                            _ => false
                        };
                    }

                    Debug.LogError($"Bool variable with key '{Key}' not found.");
                    break;

                case DialogueVariableType.Int:
                    int? intValue = DialogueVariables.GetInt(Key);

                    if (intValue != null)
                    {
                        // Swtich return according to the int comparison
                        return IntComparisonType switch
                        {
                            IntComparisonType.Equal => intValue == IntValue,
                            IntComparisonType.NotEqual => intValue != IntValue,
                            IntComparisonType.Greater => intValue > IntValue,
                            IntComparisonType.GreaterOrEqual => intValue >= IntValue,
                            IntComparisonType.Less => intValue < IntValue,
                            IntComparisonType.LessOrEqual => intValue <= IntValue,
                            _ => false
                        };
                    }

                    Debug.LogError($"Int variable with key '{Key}' not found.");
                    break;

                case DialogueVariableType.String:
                    string stringValue = DialogueVariables.GetString(Key);

                    if (stringValue != null)
                    {
                        return StringComparisonType switch
                        {
                            StringComparisonType.Equal => stringValue == StringValue,
                            StringComparisonType.NotEqual => stringValue != StringValue,
                            StringComparisonType.Contains => stringValue.Contains(StringValue),
                            StringComparisonType.StartsWith => stringValue.StartsWith(StringValue),
                            StringComparisonType.EndsWith => stringValue.EndsWith(StringValue),
                            _ => false
                        };
                    }

                    Debug.LogError($"String variable with key '{Key}' not found.");
                    break;
            }

            return false;
        }
    }
}