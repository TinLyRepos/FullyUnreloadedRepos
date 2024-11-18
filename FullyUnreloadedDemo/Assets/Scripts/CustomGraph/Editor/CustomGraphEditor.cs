using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class CustomGraphEditor : EditorWindow
{
    private static SO_NodeGraph currentNodeGraph = default;

    private GUIStyle nodeStyle_Default = default;
    private GUIStyle nodeStyle_Selected = default;

    private Vector2 graphOffset = Vector2.zero;
    private Vector2 graphDrag = Vector2.zero;

    private SO_Node selectedRoomNode = null;
    private SO_NodeTypeList roomNodeTypeList = default;

    // Node style values
    private const ushort NODE_WIDTH = 160;
    private const ushort NODE_HEIGHT = 75;
    private const ushort NODE_PADDING = 25;
    private const ushort NODE_BORDER = 12;

    // Connect line values
    private const ushort CONNECT_LINE_THICKNESS = 3;
    private const ushort CONNECT_LINE_ARROW_SIZE = 6;

    // Grid Spacing
    private const ushort GRID_LARGE = 100;
    private const ushort GRID_SMALL = 25;

    //===========================================================================
    [MenuItem("Custom Graph Editor", menuItem = "Window/Custom Editor/Custom Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<CustomGraphEditor>("Custom Graph Editor");
    }

    /// Open the room node graph editor window by double click the SO file
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        SO_NodeGraph nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as SO_NodeGraph;
        if (nodeGraph == null)
            return false;

        OpenWindow();
        currentNodeGraph = nodeGraph;
        return true;
    }

    //===========================================================================
    private void OnEnable()
    {
        // Subcribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        // Define node layout style: Default
        nodeStyle_Default = new GUIStyle();
        nodeStyle_Default.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        nodeStyle_Default.normal.textColor = Color.white;
        nodeStyle_Default.padding = new RectOffset(NODE_PADDING, NODE_PADDING, NODE_PADDING, NODE_PADDING);
        nodeStyle_Default.border = new RectOffset(NODE_BORDER, NODE_BORDER, NODE_BORDER, NODE_BORDER);

        // Define node layout style: Selected
        nodeStyle_Selected = new GUIStyle();
        nodeStyle_Selected.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        nodeStyle_Selected.normal.textColor = Color.white;
        nodeStyle_Selected.padding = new RectOffset(NODE_PADDING, NODE_PADDING, NODE_PADDING, NODE_PADDING);
        nodeStyle_Selected.border = new RectOffset(NODE_BORDER, NODE_BORDER, NODE_BORDER, NODE_BORDER);

        // Load Room Node Types
        roomNodeTypeList = GameResources.Instance.nodeTypeList;
    }

    private void OnGUI()
    {
        // If a scriptable object of type SO_NodeGraph has been selected then process
        if (currentNodeGraph != null)
        {
            // Draw Grid
            DrawBackgroundGrid(GRID_SMALL, 0.2f, Color.gray);
            DrawBackgroundGrid(GRID_LARGE, 0.2f, Color.gray);

            // Draw dragging line
            DrawDraggingLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw connect lines between nodes
            DrawNodeConnectLine();

            // Draw Room Nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    //===========================================================================
    private void DrawBackgroundGrid(ushort size, float opacity, Color color)
    {
        int lineCount_Vertical = Mathf.CeilToInt((position.width + size) / size);
        int lineCount_Horizontal = Mathf.CeilToInt((position.height + size) / size);

        Handles.color = new Color(color.r, color.g, color.b, opacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % size, graphOffset.y % size, 0);

        // Draw vertical lines
        for (ushort i = 0; i < lineCount_Vertical; ++i)
        {
            Vector3 startPos = new Vector3(size * i, -size, 0) + gridOffset;
            Vector3 endPos = new Vector3(size * i, position.height + size, 0) + gridOffset;

            Handles.DrawLine(startPos, endPos);
        }

        // Draw horizontal lines
        for (ushort i = 0; i < lineCount_Vertical; ++i)
        {
            Vector3 startPos = new Vector3(-size, size * i, 0) + gridOffset;
            Vector3 endPos = new Vector3(position.width + size, size * i, 0) + gridOffset;

            Handles.DrawLine(startPos, endPos);
        }

        Handles.color = Color.white;
    }

    private void DrawDraggingLine()
    {
        if (currentNodeGraph.endLinePosition == Vector2.zero)
            return;

        Handles.DrawBezier(
            currentNodeGraph.roomNodeStart.rect.center, currentNodeGraph.endLinePosition,
            currentNodeGraph.roomNodeStart.rect.center, currentNodeGraph.endLinePosition,
            Color.white, null, CONNECT_LINE_THICKNESS);
    }

    private void DrawNodeConnectLine()
    {
        foreach (SO_Node roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.roomNodeIDList_Child.Count == 0)
                continue;

            foreach (string childRoomNodeID in roomNode.roomNodeIDList_Child)
            {
                if (currentNodeGraph.nodeDictionary.ContainsKey(childRoomNodeID) == false)
                    continue;

                // Calculate line
                Vector2 startPos = roomNode.rect.center;
                Vector2 endPos = currentNodeGraph.nodeDictionary[childRoomNodeID].rect.center;

                Vector2 midPos = (startPos + endPos) * 0.5f;
                Vector2 direction = endPos - startPos;

                // Calculate normalize perpendicular positions from midPos
                Vector2 arrowTailPos1 = midPos - new Vector2(-direction.y, direction.x).normalized * CONNECT_LINE_ARROW_SIZE;
                Vector2 arrowTailPos2 = midPos + new Vector2(-direction.y, direction.x).normalized * CONNECT_LINE_ARROW_SIZE;
                Vector2 arrowHeadPos = midPos + direction.normalized * CONNECT_LINE_ARROW_SIZE;

                // Draw line
                Handles.DrawBezier(arrowHeadPos, arrowTailPos1, arrowHeadPos, arrowTailPos1, Color.white, null, CONNECT_LINE_THICKNESS);
                Handles.DrawBezier(arrowHeadPos, arrowTailPos2, arrowHeadPos, arrowTailPos2, Color.white, null, CONNECT_LINE_THICKNESS);
                Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, CONNECT_LINE_THICKNESS);

                GUI.changed = true;
            }
        }
    }

    private void DrawRoomNodes()
    {
        // Loop through all room nodes and draw them
        foreach (SO_Node roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(nodeStyle_Selected);
            }
            else
            {
                roomNode.Draw(nodeStyle_Default);
            }
        }

        GUI.changed = true;
    }

    //===========================================================================
    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        graphDrag = Vector2.zero;

        if (selectedRoomNode == null || selectedRoomNode.isBeingDragged == false)
        {
            selectedRoomNode = GetNodeAtMouse(currentEvent);
        }

        if (selectedRoomNode == null || currentNodeGraph.roomNodeStart != null)
        {
            ProcessNodeGraphEvents(currentEvent);
        }
        else
        {
            selectedRoomNode.ProcessEvents(currentEvent);
        }
    }

    //===========================================================================
    private void ProcessNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessEvents_MouseDown(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessEvents_MouseUp(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessEvents_MouseDrag(currentEvent);
                break;
            default:
                break;
        }
    }

    private void ProcessEvents_MouseDown(Event e)
    {
        switch (e.button)
        {
            case 0: // LEFT MOUSE
                // Clear Dragging Line
                currentNodeGraph.roomNodeStart = null;
                currentNodeGraph.endLinePosition = Vector2.zero;
                GUI.changed = true;

                ClearAllSelectedRoomNodes();
                break;
            case 1: // RIGHT MOUSE
                // Create and open context menu
                ShowContextMenu(e.mousePosition);
                break;
            default:
                break;
        }
    }

    private void ProcessEvents_MouseUp(Event e)
    {
        switch (e.button)
        {
            case 1: // RIGHT MOUSE
                if (currentNodeGraph.roomNodeStart != null)
                {
                    SO_Node roomNode = GetNodeAtMouse(e);
                    if (roomNode != null && currentNodeGraph.roomNodeStart.AddRoomNodeID_Child(roomNode.guid))
                    {   // Set room node as a child of the parent room node if possible
                        // Set parent ID in child room node
                        roomNode.AddRoomNodeID_Parent(currentNodeGraph.roomNodeStart.guid);
                    }

                    // Clear Dragging Line
                    currentNodeGraph.roomNodeStart = null;
                    currentNodeGraph.endLinePosition = Vector2.zero;
                    GUI.changed = true;
                }
                break;
            default:
                break;
        }
    }

    private void ProcessEvents_MouseDrag(Event e)
    {
        switch (e.button)
        {
            case 1: // RIGHT MOUSE
                if (currentNodeGraph.roomNodeStart != null)
                {
                    currentNodeGraph.endLinePosition += e.delta;
                    GUI.changed = true;
                }
                break;
            case 2: // MIDDLE MOUSE
                graphDrag = e.delta;
                for (int i = 0; i < currentNodeGraph.nodeList.Count; ++i)
                    currentNodeGraph.nodeList[i].DragNode(e);
                GUI.changed = true;
                break;
            default:
                break;
        }
    }

    //===========================================================================
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("New Node"), false, CreateNode, mousePosition);
        menu.AddSeparator(string.Empty);

        menu.AddItem(new GUIContent("Select All"), false, SelectAllRoomNodes);
        menu.AddSeparator(string.Empty);

        menu.AddItem(new GUIContent("Delete Selected Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Nodes"), false, DeleteSelectedRoomNodes);
        menu.ShowAsContext();
    }

    //===========================================================================
    private SO_Node GetNodeAtMouse(Event currentEvent)
    {
        for (int i = currentNodeGraph.nodeList.Count - 1; i >= 0; i--)
        {
            if (currentNodeGraph.nodeList[i].rect.Contains(currentEvent.mousePosition))
                return currentNodeGraph.nodeList[i];
        }

        return null;
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (SO_Node roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected == false)
                continue;

            roomNode.isSelected = false;
            GUI.changed = true;
        }
    }

    private void SelectAllRoomNodes()
    {
        foreach (SO_Node roomNode in currentNodeGraph.nodeList)
            roomNode.isSelected = true;

        GUI.changed = true;
    }

    // Create a room node at the mouse position
    private void CreateNode(object mousePositionObject)
    {
        // If current graph is empty then add entrance room
        if (currentNodeGraph.nodeList.Count == 0)
            CreateNode(new Vector2(200.0f, 200.0f), roomNodeTypeList.list.Find(x => x.isEntrance));

        CreateNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateNode(object mousePositionObject, SO_NodeType nodeType)
    {
        Vector2 mousePos = (Vector2)mousePositionObject;

        // create room node scriptable object
        SO_Node node = CreateInstance<SO_Node>();

        // add room node to current room node graph room node list
        currentNodeGraph.nodeList.Add(node);

        // set room node values
        node.Initialize(new Rect(mousePos, new Vector2(NODE_WIDTH, NODE_HEIGHT)), currentNodeGraph, nodeType);

        // add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(node, currentNodeGraph);
        AssetDatabase.SaveAssets();

        // Refresh graph node dictionary
        currentNodeGraph.OnValidate();
    }

    // Delete selected room nodes and links
    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (SO_Node roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected == false || roomNode.roomNodeIDList_Child.Count == 0)
                continue;

            for (int i = roomNode.roomNodeIDList_Child.Count - 1; i >= 0; i--)
            {
                // Get child room node
                SO_Node childRoomNode = currentNodeGraph.GetRoomNode(roomNode.roomNodeIDList_Child[i]);
                if (childRoomNode == null || childRoomNode.isSelected == false)
                    continue;

                roomNode.RemoveRoomNodeID_Child(childRoomNode.guid);
                childRoomNode.RemoveRoomNodeID_Parent(roomNode.guid);
            }
        }

        ClearAllSelectedRoomNodes();
    }

    private void DeleteSelectedRoomNodes()
    {
        Queue<SO_Node> roomNodeDeletionQueue = new Queue<SO_Node>();

        foreach (SO_Node roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected && roomNode.roomNodeType.isEntrance == false)
            {
                // Add room to delete queue
                roomNodeDeletionQueue.Enqueue(roomNode);

                // Break all connection reference before deletion

                // iterate through child room nodes guid
                foreach (string childRoomNodeID in roomNode.roomNodeIDList_Child)
                {
                    // Retrive child room node
                    SO_Node childRoomNode = currentNodeGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode != null)
                        childRoomNode.RemoveRoomNodeID_Parent(roomNode.guid);
                }

                // iterate through child room nodes guid
                foreach (string parentRoomNodeID in roomNode.roomNodeIDList_Parent)
                {
                    // Retrive child room node
                    SO_Node parentRoomNode = currentNodeGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                        parentRoomNode.RemoveRoomNodeID_Child(roomNode.guid);
                }
            }
        }

        while (roomNodeDeletionQueue.Count != 0)
        {
            // Get room node from queue
            SO_Node roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            // Remove node from dictionary
            currentNodeGraph.nodeDictionary.Remove(roomNodeToDelete.guid);

            // Remove node from list
            currentNodeGraph.nodeList.Remove(roomNodeToDelete);

            // Remove node from asset database
            DestroyImmediate(roomNodeToDelete, true);
        }

        AssetDatabase.SaveAssets();
    }

    // Selection changed in the Unity inspector
    private void InspectorSelectionChanged()
    {
        SO_NodeGraph roomNodeGraph = Selection.activeObject as SO_NodeGraph;
        if (roomNodeGraph == null)
            return;

        currentNodeGraph = roomNodeGraph;
        GUI.changed = true;
    }
}