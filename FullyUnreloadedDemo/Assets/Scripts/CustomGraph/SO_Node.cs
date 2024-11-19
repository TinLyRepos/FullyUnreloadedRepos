using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SO_Node : ScriptableObject
{
    [HideInInspector] public string guid = string.Empty;
    [HideInInspector] public List<string> roomNodeIDList_Parent = new List<string>();
    [HideInInspector] public List<string> roomNodeIDList_Child = new List<string>();
    [HideInInspector] public SO_NodeGraph roomNodeGraph = null;
    [HideInInspector] public SO_NodeType roomNodeType = default;
    [HideInInspector] public SO_NodeTypeList roomNodeTypeList = null;

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isBeingDragged = false;
    [HideInInspector] public bool isSelected = false;

    private GUIStyle labelStyle = default;

    //===========================================================================
    public void Initialize(Rect rect, SO_NodeGraph roomNodeGraph, SO_NodeType roomNodeType)
    {
        name = "RoomNode";

        guid = Guid.NewGuid().ToString();
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room nodeID type list
        roomNodeTypeList = GameResources.Instance.nodeTypeList;

        // Create a GUIStyle for the label
        labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.fontStyle = FontStyle.Bold;      // Make text bold
        labelStyle.normal.textColor = Color.white;  // Set text color to white
        labelStyle.alignment = TextAnchor.MiddleCenter;  // Center-align the text

        this.rect = rect;
    }

    //===========================================================================
    private string[] GetRoomNodeTypeList()
    {
        string[] roomNodeTypes = new string[roomNodeTypeList.list.Count];
        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].showInMapEditor)
                roomNodeTypes[i] = roomNodeTypeList.list[i].nodeTypeName;
        }
        return roomNodeTypes;
    }

    private bool IsChildRoomValid(string nodeID)
    {
        // Child Room Type cannot be entrance
        if (roomNodeGraph.GetRoomNode(nodeID).roomNodeType.isEntrance)
            return false;

        // Child Room Type cannot be self
        if (guid == nodeID)
            return false;

        // Child Room Type cannot be None
        if (roomNodeGraph.GetRoomNode(nodeID).roomNodeType.isNone)
            return false;

        // No Duplicate nodeID
        if (roomNodeIDList_Child.Contains(nodeID))
            return false;

        // No Circular connection
        if (roomNodeIDList_Parent.Contains(nodeID))
            return false;

        // Child Room Type is Boss Room => cannot have already connected boss room
        if (roomNodeGraph.GetRoomNode(nodeID).roomNodeType.isBossRoom)
        {
            bool isConnectedBossNodeAlready = false;
            foreach (SO_Node roomNode in roomNodeGraph.nodeList)
            {
                if (roomNode.roomNodeType.isBossRoom && roomNode.roomNodeIDList_Parent.Count != 0)
                    isConnectedBossNodeAlready = true;
            }

            if (isConnectedBossNodeAlready)
                return false;
        }

        // Child Room cannot already connected
        if (roomNodeGraph.GetRoomNode(nodeID).roomNodeIDList_Parent.Count != 0)
            return false;

        // Child room and this cannot be both corridor
        if (roomNodeGraph.GetRoomNode(nodeID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        // Child room and this cannot be both not corridor
        if (!roomNodeGraph.GetRoomNode(nodeID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        // If child room is a corridor: Check for maximum Corridor allowed for this room
        if (roomNodeGraph.GetRoomNode(nodeID).roomNodeType.isCorridor && roomNodeIDList_Child.Count >= Settings.MAX_CHILD_CORRIDORS)
            return false;

        // If child room is a corridor: Check for maximum Corridor allowed for this room
        if (!roomNodeGraph.GetRoomNode(nodeID).roomNodeType.isCorridor && roomNodeIDList_Child.Count != 0)
            return false;

        return true;
    }

    private void ProcessEvent_MouseDown(Event e)
    {
        switch (e.button)
        {
            case 0: // Left click
                Selection.activeObject = this;
                isSelected = !isSelected;
                break;
            case 1: // Right click
                roomNodeGraph.SetDrawStartNode(this, e.mousePosition);
                break;
            default:
                break;
        }
    }

    private void ProcessEvent_MouseUp(Event e)
    {
        switch (e.button)
        {
            case 0: // Left click
                if (isBeingDragged)
                    isBeingDragged = false;
                break;
            default:
                break;
        }
    }

    private void ProcessEvent_MouseDrag(Event e)
    {
        switch (e.button)
        {
            case 0: // Left click
                isBeingDragged = true;
                DragNode(e);
                GUI.changed = true;
                break;
            default:
                break;
        }
    }

    //===========================================================================
    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);

        EditorGUI.BeginChangeCheck();

        if (roomNodeIDList_Parent.Count != 0 || roomNodeType.isEntrance)
        {
            // Display the label with the custom style
            EditorGUILayout.LabelField(roomNodeType.nodeTypeName, labelStyle);
        }
        else
        {   // Create drop down selection for room type
            int selectedType = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            // Create list of options
            int selection = EditorGUILayout.Popup(string.Empty, selectedType, GetRoomNodeTypeList());

            roomNodeType = roomNodeTypeList.list[selection];

            // If the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selectedType].isCorridor && roomNodeTypeList.list[selection].isCorridor == false ||
                roomNodeTypeList.list[selectedType].isCorridor == false && roomNodeTypeList.list[selection].isCorridor ||
                roomNodeTypeList.list[selectedType].isBossRoom == false && roomNodeTypeList.list[selection].isBossRoom)
            {
                // Remove potentially invalid links
                if (roomNodeIDList_Child.Count > 0)
                {
                    for (int i = roomNodeIDList_Child.Count - 1; i >= 0; i--)
                    {
                        // Get child room node
                        SO_Node childRoomNode = roomNodeGraph.GetRoomNode(roomNodeIDList_Child[i]);
                        if (childRoomNode == null)
                            continue;

                        RemoveRoomNodeID_Child(childRoomNode.guid);
                        childRoomNode.RemoveRoomNodeID_Parent(guid);
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    public void DragNode(Event e)
    {
        rect.position += e.delta;
        EditorUtility.SetDirty(this);
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessEvent_MouseDown(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessEvent_MouseUp(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessEvent_MouseDrag(currentEvent);
                break;
            default:
                break;
        }
    }

    public bool AddRoomNodeID_Parent(string nodeID)
    {
        roomNodeIDList_Parent.Add(nodeID);
        return true;
    }

    public bool AddRoomNodeID_Child(string nodeID)
    {
        if (IsChildRoomValid(nodeID))
        {
            roomNodeIDList_Child.Add(nodeID);
            return true;
        }

        return false;
    }

    public bool RemoveRoomNodeID_Child(string nodeID)
    {
        if (roomNodeIDList_Child.Contains(nodeID))
        {
            roomNodeIDList_Child.Remove(nodeID);
            return true;
        }
        return false;
    }

    public bool RemoveRoomNodeID_Parent(string nodeID)
    {
        if (roomNodeIDList_Parent.Contains(nodeID))
        {
            roomNodeIDList_Parent.Remove(nodeID);
            return true;
        }
        return false;
    }
#endif
}