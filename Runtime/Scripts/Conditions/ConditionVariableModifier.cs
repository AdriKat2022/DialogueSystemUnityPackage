using AdriKat.DialogueSystem.Variables;
using UnityEngine;

namespace AdriKat.DialogueSystem.Conditions
{
    public class ConditionVariableModifier : MonoBehaviour
    {
        [SerializeField] private DialogueVariableNamesSO _dialogueVariablesNamesSO;

        [SerializeField] private DialogueVariableType _modifierValueType;
        [Space]
        [SerializeField] private string _boolKey;
        [SerializeField] private BoolModifierType _boolModifierType;
        [SerializeField] private bool _boolValue;
        [Space]
        [SerializeField] private string _intKey;
        [SerializeField] private IntModifierType _intModifierType;
        [SerializeField] private int _intValue;
        [Space]
        [SerializeField] private string _stringKey;
        [SerializeField] private StringModifierType _stringModifierType;
        [SerializeField] private string _stringValue;

        public void ApplyModifier()
        {
            switch (_modifierValueType)
            {
                case DialogueVariableType.Bool:
                    bool? boolValue = DialogueVariables.GetBool(_boolKey);
                    if (boolValue != null)
                    {
                        switch (_boolModifierType)
                        {
                            case BoolModifierType.Set:
                                DialogueVariables.SetBool(_boolKey, _boolValue);
                                break;
                            case BoolModifierType.Toggle:
                                DialogueVariables.SetBool(_boolKey, !(bool)boolValue);
                                break;
                            case BoolModifierType.And:
                                DialogueVariables.SetBool(_boolKey, (bool)boolValue && _boolValue);
                                break;
                            case BoolModifierType.Or:
                                DialogueVariables.SetBool(_boolKey, (bool)boolValue || _boolValue);
                                break;
                            case BoolModifierType.Xor:
                                DialogueVariables.SetBool(_boolKey, (bool)boolValue ^ _boolValue);
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Bool variable with key '{_boolKey}' not found.");
                    }
                    break;
                case DialogueVariableType.Int:
                    int? intValue = DialogueVariables.GetInt(_intKey);
                    if (intValue != null)
                    {
                        switch (_intModifierType)
                        {
                            case IntModifierType.Set:
                                DialogueVariables.SetInt(_intKey, _intValue);
                                break;
                            case IntModifierType.Add:
                                DialogueVariables.SetInt(_intKey, (int)intValue + _intValue);
                                break;
                            case IntModifierType.Subtract:
                                DialogueVariables.SetInt(_intKey, (int)intValue - _intValue);
                                break;
                            case IntModifierType.Multiply:
                                DialogueVariables.SetInt(_intKey, (int)intValue * _intValue);
                                break;
                            case IntModifierType.Divide:
                                DialogueVariables.SetInt(_intKey, (int)intValue / _intValue);
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Int variable with key '{_intKey}' not found.");
                    }
                    break;
                case DialogueVariableType.String:
                    string stringValue = DialogueVariables.GetString(_stringKey);
                    if (stringValue != null)
                    {
                        switch (_stringModifierType)
                        {
                            case StringModifierType.Set:
                                DialogueVariables.SetString(_stringKey, _stringValue);
                                break;
                            case StringModifierType.Append:
                                DialogueVariables.SetString(_stringKey, stringValue + _stringValue);
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"String variable with key '{_stringKey}' not found.");
                    }
                    break;
            }
        }

        #region Enums
        public enum BoolModifierType
        {
            Set,
            Toggle,
            And,
            Or,
            Xor
        }

        public enum IntModifierType
        {
            Set,
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public enum StringModifierType
        {
            Set,
            Append
        }
        #endregion
    }
}