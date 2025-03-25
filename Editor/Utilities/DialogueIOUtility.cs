using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Elements;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AdriKat.DialogueSystem.Utility
{
    public static class DialogueIOUtility
    {
        public static readonly string GRAPHS_SAVE_PATH = "Assets/Resources/DialogueSystem/Editor/Graphs";
        private static readonly string DIALOGUES_SAVE_PATH = "Assets/Resources/DialogueSystem";
        private static readonly string DIALOGUES_GLOBALSPACE_FOLDER = "Global";
        private static readonly string DIALOGUES_GROUPSPACE_FOLDER = "Groups";

        private static DialogueGraphView _graphView;
        private static string _graphFileName;
        private static string _dialogueContainerFolderPath;

        private static List<DialogueGroup> _groups;
        private static List<DialogueNode> _nodes;

        private static Dictionary<string, DialogueGroupSO> _createdDialogueGroups;
        private static Dictionary<string, DialogueSO> _createdDialogues;

        private static Dictionary<string, DialogueGroup> _loadedGroups;
        private static Dictionary<string, DialogueNode> _loadedNodes;

        public static void Initialize(DialogueGraphView graphView, string graphFileName)
        {
            _graphView = graphView;
            _graphFileName = graphFileName;
            _dialogueContainerFolderPath = $"{DIALOGUES_SAVE_PATH}/{_graphFileName}";

            _groups = new List<DialogueGroup>();
            _nodes = new List<DialogueNode>();
            _createdDialogueGroups = new Dictionary<string, DialogueGroupSO>();
            _createdDialogues = new Dictionary<string, DialogueSO>();
            _loadedGroups = new Dictionary<string, DialogueGroup>();
            _loadedNodes = new Dictionary<string, DialogueNode>();
        }

        #region Save Methods
        public static void Save()
        {
            CreateStaticFolders();
            GetElementsFromGraphView();

            // Save the graph data (that will be loaded when the graph is opened again)
            DialogueGraphSaveDataSO graphData = CreateAsset<DialogueGraphSaveDataSO>(GRAPHS_SAVE_PATH, $"{_graphFileName}Graph");
            graphData.Initialize(_graphFileName);

            // Save the dialogue container (for usable runtime data)
            DialogueContainerSO dialogueContainer = CreateAsset<DialogueContainerSO>(_dialogueContainerFolderPath, _graphFileName);
            dialogueContainer.Initialize(_graphFileName);

            SaveGroups(graphData, dialogueContainer);

            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }

        #region Groups
        private static void SaveGroups(DialogueGraphSaveDataSO graphData, DialogueContainerSO dialogueContainer)
        {
            List<string> groupNames = new();
            foreach (DialogueGroup group in _groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DialogueGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count > 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string group in groupsToRemove)
                {
                    RemoveFolder($"{_dialogueContainerFolderPath}/{DIALOGUES_GROUPSPACE_FOLDER}/{group}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        private static void SaveGroupToScriptableObject(DialogueGroup group, DialogueContainerSO dialogueContainer)
        {
            string groupName = group.title;
            CreateFolderRecursive(_dialogueContainerFolderPath, $"{DIALOGUES_GROUPSPACE_FOLDER}/{groupName}/Dialogues");

            DialogueGroupSO dialogueGroup = CreateAsset<DialogueGroupSO>($"{_dialogueContainerFolderPath}/{DIALOGUES_GROUPSPACE_FOLDER}/{groupName}", groupName);
            dialogueGroup.Initialize(groupName);
            _createdDialogueGroups.Add(group.ID, dialogueGroup);
            dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<DialogueSO>());

            SaveAsset(dialogueGroup);
        }

        private static void SaveGroupToGraph(DialogueGroup group, DialogueGraphSaveDataSO graphData)
        {
            DialogueGroupSaveData groupSaveData = new()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };
            graphData.Groups.Add(groupSaveData);
        }
        #endregion

        #region Nodes

        private static void SaveNodes(DialogueGraphSaveDataSO graphData, DialogueContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new();
            List<string> ungroupedNodeNames = new();

            foreach (DialogueNode node in _nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);

                if (node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                }
                else
                {
                    ungroupedNodeNames.Add(node.DialogueName);
                }
            }

            UpdateDialogueChoicesConnections();
            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DialogueGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count > 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = oldGroupNode.Value.Except(currentGroupedNodeNames[oldGroupNode.Key]).ToList();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupNode.Key))
                    {
                        nodesToRemove = oldGroupNode.Value.Except(currentGroupedNodeNames[oldGroupNode.Key]).ToList();
                    }

                    foreach (string node in nodesToRemove)
                    {
                        RemoveAsset($"{_dialogueContainerFolderPath}/Groups/{oldGroupNode.Key}/Dialogues", node);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DialogueGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count > 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();
                foreach (string node in nodesToRemove)
                {
                    RemoveAsset($"{_dialogueContainerFolderPath}/Global/Dialogues", node);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }

        private static void SaveNodeToGraph(DialogueNode node, DialogueGraphSaveDataSO graphData)
        {
            DialogueNodeSaveData nodeSaveData = new()
            {
                ID = node.ID,
                Name = node.DialogueName,
                Choices = CloneChoices(node.Choices),
                Text = node.DialogueText,
                Type = node.Type,
                GroupID = node.Group?.ID,
                Position = node.GetPosition().position
            };

            if (node is DialogueConditionalBranchNode conditionalBranchNode)
            {
                nodeSaveData.DialogueVariableNames = conditionalBranchNode.DialogueVariableNames;
                nodeSaveData.Conditions = CloneConditions(conditionalBranchNode.Conditions);
                nodeSaveData.ConditionsToBeMet = conditionalBranchNode.ConditionToBeMet;
                nodeSaveData.NodeIDOnTrue = conditionalBranchNode.NodeOnTrue;
                nodeSaveData.NodeIDOnFalse = conditionalBranchNode.NodeOnFalse;
                Debug.Log($"Saving to graph with: true: {nodeSaveData.NodeIDOnTrue}/false: {nodeSaveData.NodeIDOnFalse}");
            }

            graphData.Nodes.Add(nodeSaveData);
        }

        private static void SaveNodeToScriptableObject(DialogueNode node, DialogueContainerSO dialogueContainer)
        {
            if (node.Type == DialogueType.ConditionalBranch)
            {
                SaveConditionalNodeToScriptableObject(node, dialogueContainer);
            }
            else
            {
                SaveExecutableNodeToScriptableObject(node, dialogueContainer);
            }
        }

        private static void SaveExecutableNodeToScriptableObject(DialogueNode node, DialogueContainerSO dialogueContainer)
        {
            ExecutableDialogueSO dialogue = SaveDialogue<ExecutableDialogueSO>(node, dialogueContainer);

            dialogue.Initialize(
                node.DialogueName,
                node.DialogueText,
                ConvertNodeChoicesToDialogueChoices(node.Choices),
                node.Type,
                node.IsStartingNode()
            );

            SaveAsset(dialogue);
            _createdDialogues.Add(node.ID, dialogue);
        }

        private static void SaveConditionalNodeToScriptableObject(DialogueNode node, DialogueContainerSO dialogueContainer)
        {
            DialogueConditionalBranchSO dialogue = SaveDialogue<DialogueConditionalBranchSO>(node, dialogueContainer);


            if (node is DialogueConditionalBranchNode conditionalBranchNode)
            {
                dialogue.Initialize(
                    CloneConditions(conditionalBranchNode.Conditions),
                    conditionalBranchNode.ConditionToBeMet,
                    null,
                    null,
                    node.IsStartingNode()
                );
                dialogue.DialogueName = conditionalBranchNode.DialogueName;
                dialogue.DialogueVariableNames = conditionalBranchNode.DialogueVariableNames;
            }

            _createdDialogues.Add(node.ID, dialogue);
            SaveAsset(dialogue);
        }

        private static List<DialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DialogueChoiceSaveData> choices)
        {
            List<DialogueChoiceData> dialogueChoices = new();

            foreach (DialogueChoiceSaveData choice in choices)
            {
                dialogueChoices.Add(new DialogueChoiceData
                {
                    Text = choice.Text,
                });
            }

            return dialogueChoices;
        }

        private static void UpdateDialogueChoicesConnections()
        {
            foreach (var node in _nodes)
            {
                DialogueSO dialogue = _createdDialogues[node.ID];

                Debug.Log(node.ID);

                if (dialogue is DialogueConditionalBranchSO conditionalBranchSO)
                {
                    DialogueConditionalBranchNode conditionalBranchNode = node as DialogueConditionalBranchNode;

                    Debug.Log($"Node on true: {conditionalBranchNode.NodeOnTrue}");
                    if (!string.IsNullOrEmpty(conditionalBranchNode.NodeOnTrue))
                    {
                        conditionalBranchSO.DialogueOnTrue = _createdDialogues[conditionalBranchNode.NodeOnTrue];
                    }

                    Debug.Log($"Node on false: {conditionalBranchNode.NodeOnTrue}");
                    if (!string.IsNullOrEmpty(conditionalBranchNode.NodeOnFalse))
                    {
                        conditionalBranchSO.DialogueOnFalse = _createdDialogues[conditionalBranchNode.NodeOnFalse];
                    }

                    conditionalBranchSO.DialogueVariableNames = conditionalBranchNode.DialogueVariableNames;

                    SaveAsset(dialogue);
                }
                else if (dialogue is ExecutableDialogueSO executableDialogueSO)
                {
                    for (int choiceIndex = 0; choiceIndex < node.Choices.Count; choiceIndex++)
                    {
                        DialogueChoiceSaveData choice = node.Choices[choiceIndex];

                        if (string.IsNullOrEmpty(choice.NodeID))
                        {
                            continue;
                        }

                        executableDialogueSO.Choices[choiceIndex].NextDialogue = _createdDialogues[choice.NodeID];
                        SaveAsset(dialogue);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Load Methods

        public static void Load()
        {
            DialogueGraphSaveDataSO graphData = LoadAsset<DialogueGraphSaveDataSO>(GRAPHS_SAVE_PATH, _graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Graph Load Error",
                    "No graph data found!\n" +
                    $"\"{GRAPHS_SAVE_PATH}/{_graphFileName}\"\n\n" +
                    "Make sure the graph data you are trying to load is placed in \"Assets/Resources/DialogueSystem/Editor/Graphs\".",
                    "Ok"
                );
                return;
            }

            Debug.Log($"Loading graph {_graphFileName}...");

            DialogueEditorWindow.UpdateFileName(graphData.FileName);
            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();

            Debug.Log($"Graph {_graphFileName} loaded successfully.");
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, DialogueNode> loadedNode in _loadedNodes)
            {
                foreach (Port port in loadedNode.Value.outputContainer.Children())
                {
                    Port nexNodeInputPort;

                    if (port.userData is DialogueChoiceSaveData choiceData)
                    {
                        if (string.IsNullOrEmpty(choiceData.NodeID))
                        {
                            continue;
                        }

                        DialogueNode nextNode = _loadedNodes[choiceData.NodeID];
                        nexNodeInputPort = nextNode.inputContainer.Children().First() as Port;
                    }
                    else if (port.userData is string dialogueId)
                    {
                        if (string.IsNullOrEmpty(dialogueId))
                        {
                            continue;
                        }

                        Debug.Log($"Linking conditional branch node ({dialogueId})");
                        DialogueNode nextNode = _loadedNodes[dialogueId];
                        nexNodeInputPort = nextNode.inputContainer.Children().First() as Port;
                    }
                    else if (port.userData == null)
                    {
                        continue;
                    }
                    else
                    {
                        Debug.LogError($"Error: Unable to load node connections. UserData of port unrecognized: {port.userData}");
                        continue;
                    }

                    Edge edge = port.ConnectTo(nexNodeInputPort);
                    _graphView.AddElement(edge);
                    loadedNode.Value.RefreshPorts();
                }
            }
        }

        private static void LoadNodes(List<DialogueNodeSaveData> nodes)
        {
            if (nodes == null)
            {
                Debug.LogError("Error: Unable to load nodes. Nodes is null.");
                return;
            }

            foreach (DialogueNodeSaveData nodeData in nodes)
            {
                DialogueNode node = _graphView.CreateNode(nodeData.Name, nodeData.Type, nodeData.Position, false);

                node.ID = nodeData.ID;

                if (node is DialogueConditionalBranchNode conditionalBranchNode)
                {
                    conditionalBranchNode.DialogueVariableNames = nodeData.DialogueVariableNames;
                    conditionalBranchNode.Conditions = CloneConditions(nodeData.Conditions);
                    conditionalBranchNode.ConditionToBeMet = nodeData.ConditionsToBeMet;
                    conditionalBranchNode.NodeOnTrue = nodeData.NodeIDOnTrue;
                    conditionalBranchNode.NodeOnFalse = nodeData.NodeIDOnFalse;
                }
                else if (node is DialogueNode executableNode)
                {
                    executableNode.Choices = CloneChoices(nodeData.Choices);
                    executableNode.DialogueText = nodeData.Text;
                }

                node.Draw();
                _graphView.AddElement(node);
                _loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                DialogueGroup group = _loadedGroups[nodeData.GroupID];
                node.Group = group;
                group.AddElement(node);
            }
        }

        private static void LoadGroups(List<DialogueGroupSaveData> groups)
        {
            if (groups == null)
            {
                Debug.LogError("Error: Unable to load groups. Groups is null.");
                return;
            }

            foreach (DialogueGroupSaveData groupData in groups)
            {
                DialogueGroup group = _graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                _loadedGroups.Add(group.ID, group);
            }
        }

        #endregion

        #region Fetch Methods
        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(DialogueGroup);
            _graphView.graphElements.ForEach(element =>
            {
                if (element is DialogueNode dialogueNode)
                {
                    _nodes.Add(dialogueNode);
                    return;
                }

                if (element.GetType() == groupType)
                {
                    DialogueGroup group = (DialogueGroup)element;
                    _groups.Add(group);
                    return;
                }

            });
        }

        #endregion

        #region Creation Methods
        private static void CreateStaticFolders()
        {
            // Create the folders where the graph data will be saved
            CreateFolderRecursive(GRAPHS_SAVE_PATH);

            // Create the folders where the dialogues will be saved
            CreateFolderRecursive(_dialogueContainerFolderPath);
            CreateFolderRecursive(_dialogueContainerFolderPath, DIALOGUES_GROUPSPACE_FOLDER);
            CreateFolderRecursive(_dialogueContainerFolderPath, DIALOGUES_GLOBALSPACE_FOLDER + "/Dialogues");
        }
        #endregion

        #region Utility Methods

        private static T SaveDialogue<T>(DialogueNode node, DialogueContainerSO dialogueContainer) where T : DialogueSO
        {
            T dialogue;
            if (node.Group != null)
            {
                dialogue = CreateAsset<T>(
                    $"{_dialogueContainerFolderPath}/{DIALOGUES_GROUPSPACE_FOLDER}/{node.Group.title}/Dialogues", node.DialogueName
                    );
                dialogueContainer.DialogueGroups.AddItem(_createdDialogueGroups[node.Group.ID], dialogue);
            }
            else
            {
                dialogue = CreateAsset<T>(
                    $"{_dialogueContainerFolderPath}/{DIALOGUES_GLOBALSPACE_FOLDER}/Dialogues", node.DialogueName);
                dialogueContainer.UngroupedDialogues.Add(dialogue);
            }

            return dialogue;
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        public static void CreateFolder(string path, string foldername)
        {
            if (foldername == "")
            {
                Debug.LogWarning("Error: Unable to create folder. Folder name is empty.");
                return;
            }

            string basePathName = string.IsNullOrEmpty(path) ? "" : $"{path}/";

            if (AssetDatabase.IsValidFolder($"{basePathName}{foldername}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, foldername);
        }

        public static void CreateFolderRecursive(string basePath, string foldername)
        {
            if (basePath != "" && !AssetDatabase.IsValidFolder(basePath))
            {
                Debug.LogWarning("Error: Unable to create folder while recursive creation. Base path does not exist.");
                return;
            }

            string[] folders = foldername.Split('/');
            string currentPath = basePath;
            foreach (string folder in folders)
            {
                if (folder == "")
                {
                    Debug.LogWarning("Error: Unable to create folder while recursive creation. Folder name is empty.");
                    return;
                }
                CreateFolder(currentPath, folder);
                currentPath = $"{currentPath}/{folder}";
            }
        }

        public static void CreateFolderRecursive(string foldername)
        {
            string[] folders = foldername.Split('/');
            string currentPath = "";
            foreach (string folder in folders)
            {
                if (folder == "")
                {
                    Debug.LogWarning("Error: Unable to create folder while recursive creation. Folder name is empty.");
                    return;
                }

                CreateFolder(currentPath, folder);

                if (currentPath == "")
                {
                    currentPath = folder;
                }
                else
                {
                    currentPath = $"{currentPath}/{folder}";
                }
            }
        }

        public static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            return AssetDatabase.LoadAssetAtPath<T>($"{path}/{assetName}.asset");
        }

        private static List<DialogueChoiceSaveData> CloneChoices(List<DialogueChoiceSaveData> list)
        {
            if (list == null)
            {
                Debug.LogError("Error: Unable to copy choices. Choices is null.");
                return new List<DialogueChoiceSaveData>();
            }

            List<DialogueChoiceSaveData> clonedElements = new();

            // Copy choices to avoid reference issues (the serialized object need to be updated only when saving the graph)
            foreach (var choice in list)
            {
                clonedElements.Add(new DialogueChoiceSaveData
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                });
            }

            return clonedElements;
        }

        private static List<DialogueConditionData> CloneConditions(List<DialogueConditionData> list)
        {
            if (list == null)
            {
                Debug.LogError("Error: Unable to copy choices. Choices is null.");
                return new List<DialogueConditionData>();
            }

            List<DialogueConditionData> clonedElements = new();

            // Copy choices to avoid reference issues (the serialized object need to be updated only when saving the graph)
            foreach (var condition in list)
            {
                clonedElements.Add(new DialogueConditionData
                {
                    DialogueVariablesNamesSO = condition.DialogueVariablesNamesSO,
                    ConditionValueType = condition.ConditionValueType,
                    Key = condition.Key,
                    BoolComparisonType = condition.BoolComparisonType,
                    BoolValue = condition.BoolValue,
                    IntComparisonType = condition.IntComparisonType,
                    IntValue = condition.IntValue,
                    StringComparisonType = condition.StringComparisonType,
                    StringValue = condition.StringValue
                });
            }

            return clonedElements;
        }
        #endregion
    }
}