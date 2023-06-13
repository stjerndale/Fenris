using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace DS.Utilities
{
    using Data.Save;
    using Data;
    using Elements;
    using ScriptableObjects;
    using Windows;

    public static class DSIOUtility
    {
        private static DSGraphView graphView;
        private static string graphFileName;
        private static string containerFolderPath;

        private static List<DSGroup> groups;
        private static List<DSNode> nodes;
        private static Dictionary<string, DSDialogueGroupSO> createdDialogueGroups;
        private static Dictionary<string, DSDialogueSO> createdDialogues;

        private static Dictionary<string, DSGroup> loadedGroups;
        private static Dictionary<string, DSNode> loadedNodes;

        public static void Initialize(DSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;
            graphFileName = graphName;
            containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphFileName}";

            nodes = new List<DSNode>();
            groups = new List<DSGroup>();
            createdDialogueGroups = new Dictionary<string, DSDialogueGroupSO> ();
            createdDialogues = new Dictionary<string, DSDialogueSO> ();
            loadedGroups = new Dictionary<string, DSGroup>();
            loadedNodes = new Dictionary<string, DSNode>();
        }

        #region Save Methods
        public static void Save()
        {
            CreateStaticFolders();

            GetElementsFromGraphView();

            DSGraphSaveDataSO graphData = CreateAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");
            graphData.Initialize(graphFileName);

            DSDialogueContainerSO dialogContainer = CreateAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);
            dialogContainer.Initialize(graphFileName);

            SaveGroups(graphData, dialogContainer);
            SaveNodes(graphData, dialogContainer);

            SaveAsset(graphData);
            SaveAsset(dialogContainer);
        }

        #region Groups
        private static void SaveGroups(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (DSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveDataSO graphData)
        {
            DSGroupSaveData groupData = new DSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(DSGroup group, DSDialogueContainerSO dialogContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

            DSDialogueGroupSO dialogueGroup = CreateAsset<DSDialogueGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);
            dialogueGroup.Initialize(groupName);

            createdDialogueGroups.Add(group.ID, dialogueGroup);
            dialogContainer.DialogueGroups.Add(dialogueGroup, new List<DSDialogueSO>());

            SaveAsset(dialogueGroup);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DSGraphSaveDataSO graphData)
        {
            if(graphData.OldGroupNames != null && graphData.OldGroupNames.Count > 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach(string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames); // cope the values, not the reference
        }
        #endregion

        #region Nodes
        private static void SaveNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();
            foreach(DSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);

                if(node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                }
                else
                {
                    ungroupedNodeNames.Add(node.DialogueName);
                }
            }

            UpdateDialoguesChoicesConnection();
            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(DSNode node, DSGraphSaveDataSO graphData)
        {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            DSNodeSaveData nodeData = new DSNodeSaveData()
            {
                ID = node.ID,
                Name = node.DialogueName,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                DialogueType = node.DialogueType,
                Position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(DSNode node, DSDialogueContainerSO dialogueContainer)
        {
            DSDialogueSO dialogue;
            if(node.Group != null)
            {
                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
                dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], dialogue);
            }
            else
            {
                dialogue = CreateAsset<DSDialogueSO>($"{containerFolderPath}/Global/Dialogues", node.DialogueName);
                dialogueContainer.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                node.DialogueName,
                node.Text,
                ConvertNodeChoicesToDialogueChoices(node.Choices),
                node.DialogueType,
                node.IsStartingNode()
                );

            createdDialogues.Add(node.ID, dialogue);

            SaveAsset(dialogue);
        }

        private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSDialogueChoiceData> dialogueChoices = new List<DSDialogueChoiceData>();

            foreach(DSChoiceSaveData nodeChoice in nodeChoices)
            {
                DSDialogueChoiceData choiceData = new DSDialogueChoiceData()
                {
                    Text = nodeChoice.Text
                };
                dialogueChoices.Add(choiceData);
            }

            return dialogueChoices;
        }

        private static void UpdateDialoguesChoicesConnection()
        {
            foreach(DSNode node in nodes)
            {
                DSDialogueSO dialogue = createdDialogues[node.ID];

                for(int index = 0; index < node.Choices.Count; ++index)
                {
                    DSChoiceSaveData nodeChoice = node.Choices[index];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }
                    else
                    {
                        dialogue.Choices[index].NextDialogue = createdDialogues[nodeChoice.NodeID];

                        SaveAsset(dialogue);
                    }
                }
            }
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if(graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count > 0)
            {
                foreach(KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach(string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if(graphData.OldGroupNames != null && graphData.OldUngroupedNodeNames?.Count > 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach(string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }
        #endregion

        #endregion

        #region Load Methods
        public static void Load()
        {
            DSGraphSaveDataSO graphData = LoadAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", graphFileName);
            if(graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not load the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"Assets/Editor/DialogueSystem/Graphs/{graphFileName}\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path abovve.",
                    "Okay");
                return;
            }
            else
            {
                DSEditorWindow.UpdateFileName(graphData.FileName);
                LoadGroups(graphData.Groups);
                LoadNodes(graphData.Nodes);
                LoadNodeConnections();
            }
        }

        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach(DSGroupSaveData groupData in groups)
            {
                DSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);
                group.ID = groupData.ID;
                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<DSNodeSaveData> nodes)
        {
            foreach(DSNodeSaveData nodeData in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);
                DSNode node = graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);
                
                node.ID = nodeData.ID;
                node.Choices = choices;
                node.Text = nodeData.Text;

                node.Draw();
                graphView.AddElement(node);
                loadedNodes.Add(node.ID, node);

                if (!string.IsNullOrEmpty(nodeData.GroupID))
                {
                    DSGroup group = loadedGroups[nodeData.GroupID];
                    group.AddElement(node);
                }
            }
        }

        private static void LoadNodeConnections()
        {
            foreach(KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                foreach(Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) choicePort.userData;
                    if(!string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        DSNode nextNode = loadedNodes[choiceData.NodeID];
                        Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                        Edge edge = choicePort.ConnectTo(nextNodeInputPort);
                        graphView.AddElement(edge);

                        loadedNode.Value.RefreshPorts();
                    }
                }
            }
        }
        #endregion

        #region Creation Methods
        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");

            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");

            CreateFolder("Assets/DialogueSystem/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
        }
        #endregion

        #region Fetching Methods
        private static void GetElementsFromGraphView()
        {
            graphView.graphElements.ForEach(graphElement =>
            {
                if(graphElement is DSNode node)
                {
                    nodes.Add(node);
                }
                else if(graphElement is DSGroup group)
                {
                    groups.Add(group);
                }
            });
        }
        #endregion

        #region Utility Methods
        public static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return; // folder already exists
            }
            else
            {
                AssetDatabase.CreateFolder(path, folderName);
            }
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
            string fullPath = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();
            foreach (DSChoiceSaveData choice in nodeChoices)
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID,
                };

                choices.Add(choiceData);
            }

            return choices;
        }
        #endregion
    }

}