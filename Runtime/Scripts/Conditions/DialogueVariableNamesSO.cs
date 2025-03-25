using AdriKat.DialogueSystem.Variables;
using UnityEngine;

namespace AdriKat.DialogueSystem
{

    [CreateAssetMenu(fileName = "DialogueVariableNames", menuName = "Dialogue/Conditions Names", order = 1)]
    public class DialogueVariableNamesSO : ScriptableObject
    {
        [field: SerializeField] public string[] BoolVarNames { get; private set; }
        [field: SerializeField] public string[] IntVarNames { get; private set; }
        [field: SerializeField] public string[] StringVarNames { get; private set; }

        public string[] GetVarNames(DialogueVariableType valueType)
        {
            return valueType switch
            {
                DialogueVariableType.Bool => BoolVarNames,
                DialogueVariableType.Int => IntVarNames,
                DialogueVariableType.String => StringVarNames,
                _ => null,
            };
        }
    }
}