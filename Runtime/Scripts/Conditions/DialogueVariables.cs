using System.Collections.Generic;

namespace AdriKat.DialogueSystem.Variables
{
    public static class DialogueVariables
    {
        private static readonly Dictionary<string, bool> _boolVariables = new();
        private static readonly Dictionary<string, int> _intVariables = new();
        private static readonly Dictionary<string, string> _stringVariables = new();

        #region Booleans
        public static bool? GetBool(string name)
        {
            return _boolVariables.TryGetValue(name, out bool value) ? value : null;
        }

        public static void SetBool(string name, bool value)
        {
            _boolVariables[name] = value;
        }
        #endregion

        #region Ints
        public static int? GetInt(string name)
        {
            return _intVariables.TryGetValue(name, out int value) ? value : null;
        }

        public static void SetInt(string name, int value)
        {
            _intVariables[name] = value;
        }
        #endregion

        #region Strings
        public static string GetString(string name)
        {
            return _stringVariables.TryGetValue(name, out string value) ? value : null;
        }

        public static void SetString(string name, string value)
        {
            _stringVariables[name] = value;
        }
        #endregion

        public static void ClearAllVariables()
        {
            _boolVariables.Clear();
            _intVariables.Clear();
        }

        public static void SetDialogueVariablesNamesSO(DialogueVariableNamesSO dialogueVariablesNamesSO)
        {
            foreach (var variable in dialogueVariablesNamesSO.BoolVarNames)
            {
                SetBool(variable, false);
            }

            foreach (var variable in dialogueVariablesNamesSO.IntVarNames)
            {
                SetInt(variable, 0);
            }

            foreach (var variable in dialogueVariablesNamesSO.StringVarNames)
            {
                SetString(variable, string.Empty);
            }
        }
    }

    public enum DialogueVariableType
    {
        Bool,
        Int,
        String
    }
    public enum BoolComparisonType
    {
        Is,
        And,
        Or,
        Xor
    }

    public enum IntComparisonType
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual
    }

    public enum StringComparisonType
    {
        Equal,
        NotEqual,
        Contains,
        StartsWith,
        EndsWith
    }
}