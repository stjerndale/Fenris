using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DS.Windows
{
    using Data.Error;
    using Data.Save;
    using Elements;
    using Enumerations;
    using Utilities;
    using static UnityEngine.GraphicsBuffer;

    public class DSGraphView : GraphView
    {
        private DSEditorWindow editorWindow;
        private DSSearchWindow searchWindow;

        private SerializableDictionary<string, DSNodeErrorData> ungroupedNodes;
        private SerializableDictionary<string, DSGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>> groupedNodes;

        private MiniMap miniMap;

        private int nameErrorsAmount;
        public int NameErrorsAmount
        {
            get { return nameErrorsAmount; } 
            set 
            { 
                nameErrorsAmount = value; 
                if(nameErrorsAmount == 0)
                {
                    // enable save button
                    editorWindow.EnableSaving();
                }
                else if(nameErrorsAmount == 1)
                {
                    // we got an error, so disable the save button
                    editorWindow.DisableSaving();
                }
            }
        }

        public DSGraphView(DSEditorWindow dsEditorWindow) 
        {
            editorWindow = dsEditorWindow;

            ungroupedNodes = new SerializableDictionary<string, DSNodeErrorData>();
            groups = new SerializableDictionary<string, DSGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>>();

            AddManipulators();
            AddSearchWindow();
            AddMiniMap();
            AddGridBackground();

            // setting up overriding callback methods
            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupedElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyle();
        }

        #region Overrided Methods
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) // do not connect to itself
                {
                    return;
                }
                if (startPort.node == port.node) // do not connect to its own node
                {
                    return;
                }
                if (startPort.direction == port.direction) // do not connect to ports of the some direction/type
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        #endregion

        #region Elements Addition
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "Dialogue System/DSGraphViewStyles.uss",
                "Dialogue System/DSNodeStyles.uss"
                );
        }

        private void AddSearchWindow()
        {
            if( searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
                searchWindow.Initialize(this);
            }
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddMiniMap()
        {
            miniMap = new MiniMap()
            {
                anchored = true,
            };

            miniMap.SetPosition(new Rect(15, 50, 200, 100));
            Add(miniMap);

            miniMap.visible = false;
        }

        private void AddMiniMapStyle()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
        }
        #endregion

        #region Manipulators
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger()); // must be added before the RectangleSelector in order to work
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DSDialogueType.MultipleChoice));
            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("DialogueName", dialogueType, GetLocalMousePosition( actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition( actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }
        #endregion

        #region Elements Creation
        public DSNode CreateNode(string nodeName, DSDialogueType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(nodeName, this, position);
            if(shouldDraw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

            return node;
        }

        public DSGroup CreateGroup(string title, Vector2 localMousePosition)
        {
            DSGroup group = new DSGroup(title, localMousePosition);

            AddGroup(group);
            AddElement(group);
            
            foreach(GraphElement selectedElement in selection)
            {
                if(!(selectedElement is DSNode))
                {
                    continue;
                }

                DSNode node = (DSNode) selectedElement;
                group.AddElement(node);
            }
            
            return group;
        }
        #endregion

        #region Callbacks

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DSGroup);
                Type edgeType = typeof(Edge);
                List<DSGroup> groupsToDelete = new List<DSGroup>();
                List<Edge> edgesToDelete = new List<Edge>();
                List<DSNode> nodesToDelete = new List<DSNode>();

                foreach(GraphElement element in selection)
                { // "is" checks if the element is a DSNode or a class that inherits from DSNode
                    if (element is DSNode node) // writing "node" after uses pattern matching. node becomes the element cast as a DSNode
                    {
                        nodesToDelete.Add(node);
                        continue;
                    }
                    else if (element.GetType() == edgeType)
                    {
                        Edge edge = (Edge) element;
                        edgesToDelete.Add(edge);
                        continue;
                    }
                    else if(element.GetType() == groupType)
                    {
                        DSGroup group = (DSGroup) element;
                        groupsToDelete.Add(group);
                    }
                }
                // remove the edges meant to be deleted
                DeleteElements(edgesToDelete);
                // remove the groups that are meant to be deleted
                foreach(DSGroup group in groupsToDelete)
                {
                    List<DSNode> groupedNodesToMove = new List<DSNode>();

                    foreach(GraphElement groupElement in group.containedElements)
                    {
                        if (groupElement is DSNode node)
                        {
                            groupedNodesToMove.Add(node);
                        }
                    }
                    group.RemoveElements(groupedNodesToMove);
                    RemoveGroup(group);
                    RemoveElement(group);
                }

                // remove the nodes that are meant to be deleted
                foreach (DSNode node in nodesToDelete)
                {
                    if(node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }
                    RemoveUngroupedNode(node);
                    node.DisconnectAllPorts();
                    RemoveElement(node);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach(GraphElement element in elements)
                {
                    if(!(element is DSNode))
                    {
                        continue;
                    }
                    else
                    {
                        DSGroup nodeGroup = (DSGroup) group;
                        DSNode node = (DSNode) element;
                        RemoveUngroupedNode(node);
                        AddGroupedNode(node, nodeGroup);
                    }
                }
            };
        }

        private void OnGroupedElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }
                    else
                    {
                        DSNode node = (DSNode)element;
                        // Remove node from its current group
                        RemoveGroupedNode(node, (DSGroup) group);
                        AddUngroupedNode(node);
                    }
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DSGroup dsGroup = (DSGroup) group;
                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        ++NameErrorsAmount; // a new error appeared
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        --NameErrorsAmount; // there was an error that is now fixed
                    }
                }

                RemoveGroup(dsGroup);
                dsGroup.OldTitle = dsGroup.title;
                AddGroup(dsGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if(changes.edgesToCreate != null)
                {
                    foreach(Edge edge in changes.edgesToCreate)
                    {
                        DSNode nextNode = (DSNode) edge.input.node;
                        DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;
                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if(changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach(Edge element in changes.elementsToRemove)
                    {
                        if(element.GetType() == edgeType)
                        {
                            Edge edge = (Edge) element;
                            DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;
                            choiceData.NodeID = "";
                        }
                    }
                }

                return changes;
            };
        }
        #endregion

        #region Repeated Elements Handling

        public void AddUngroupedNode(DSNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                ungroupedNodes.Add(nodeName, nodeErrorData);

                return;
            }
            else
            {
                List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

                ungroupedNodesList.Add(node);
                Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;
                node.SetErrorStyle(errorColor);
                if (ungroupedNodesList.Count == 2)
                {
                    NameErrorsAmount++;
                    ungroupedNodesList[0].SetErrorStyle(errorColor);
                }
            }
        }

        public void RemoveUngroupedNode(DSNode node)
        {
            string nodeName = node.DialogueName.ToLower();
            List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Remove(node);
            node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                NameErrorsAmount--;
                ungroupedNodesList[0].ResetStyle();
            }
            else if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }

        public void AddGroupedNode(DSNode node, DSGroup group)
        {
            string nodeName = node.DialogueName.ToLower();
            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, DSNodeErrorData>());
            }
            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();
                nodeErrorData.Nodes.Add(node);
                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }
            else
            {
                List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;
                groupedNodesList.Add(node);

                Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;
                node.SetErrorStyle(errorColor);

                if (groupedNodesList.Count == 2)
                {
                    NameErrorsAmount++;
                    groupedNodesList[0].SetErrorStyle(errorColor);
                }
            }
        }

        public void RemoveGroupedNode(DSNode node, DSGroup group)
        {
            string nodeName = node.DialogueName.ToLower();
            node.Group = null;
            List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);
            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                NameErrorsAmount--;

                groupedNodesList[0].ResetStyle();

                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }

        private void AddGroup(DSGroup group)
        {
            string groupName = group.title.ToLower();
            if(!groups.ContainsKey(groupName))
            {
                DSGroupErrorData groupErrorData = new DSGroupErrorData();
                groupErrorData.Groups.Add(group);
                groups.Add(groupName, groupErrorData);
                return;
            }
            else
            {
                List<DSGroup> groupsList = groups[groupName].Groups;
                groupsList.Add(group);
                Color errorColor = groups[groupName].ErrorData.Color;
                group.SetErrorStyle(errorColor);

                if(groupsList.Count == 2)
                {
                    NameErrorsAmount++;
                    groupsList[0].SetErrorStyle(errorColor);
                }
            }
        }

        private void RemoveGroup(DSGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();
            List<DSGroup> groupsList = groups[oldGroupName].Groups;

            groupsList.Remove(group);
            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                NameErrorsAmount--;
                groupsList[0].ResetStyle();
            }
            else if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }
        #endregion

        #region Utilities
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));

            groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();

            NameErrorsAmount = 0;
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }
        #endregion
    }
}
