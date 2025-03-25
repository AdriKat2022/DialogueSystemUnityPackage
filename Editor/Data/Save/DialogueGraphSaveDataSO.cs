using AdriKat.DialogueSystem.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    public class DialogueGraphSaveDataSO : ScriptableObject
    {
        [field: Header("WARNING: THIS IS NOT MEANT TO BE MODIFIED MANUALLY")]
        [field: Header("ALTER AT YOUR OWN RISK")]

        [field: Space(5)]
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<DialogueGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<DialogueNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Groups = new List<DialogueGroupSaveData>();
            Nodes = new List<DialogueNodeSaveData>();
        }
    }
}