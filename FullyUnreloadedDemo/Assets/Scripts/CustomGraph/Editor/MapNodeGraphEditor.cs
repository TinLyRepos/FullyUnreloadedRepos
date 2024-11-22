using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using MapNodeGraph;

public class MapNodeGraphEditor : EditorWindow
{
    private static SO_MapNodeGraph currentNodeGraph = default;

    private Dictionary<MapGraph.NODEKEY, GUIStyle> nodeStyleDictionary = new Dictionary<MapGraph.NODEKEY, GUIStyle>();

    private Vector2 graphOffset = Vector2.zero;
    private Vector2 dragDelta = Vector2.zero;

    private SO_MapNode selectedRoomNode = null;
    private SO_MapNodeTypeList roomNodeTypeList = default;

    //===========================================================================
    [MenuItem("Map Node Graph Editor", menuItem = "Window/Custom Window/Map Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<MapNodeGraphEditor>("Custom Graph Editor");
    }

    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        // Open the room node graph editor window by double click the SO file
        SO_MapNodeGraph nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as SO_MapNodeGraph;
        if (nodeGraph == null)
            return false;

        OpenWindow();
        currentNodeGraph = nodeGraph;
        return true;
    }

    //===========================================================================
    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        // Initialize the node style dictionary
        nodeStyleDictionary = new Dictionary<MapGraph.NODEKEY, GUIStyle>();

        for (int i = 0; i < MapGraph.NODE_TEXTURES.Length; i++)
        {
            // Create the node style
            GUIStyle nodeStyle = new GUIStyle
            {
                normal = {
                background = EditorGUIUtility.Load(MapGraph.NODE_TEXTURES[i]) as Texture2D,
                textColor = Color.white
            },
                padding = new RectOffset(MapGraph.NODE_PADDING, MapGraph.NODE_PADDING, MapGraph.NODE_PADDING, MapGraph.NODE_PADDING),
                border = new RectOffset(MapGraph.NODE_BORDER, MapGraph.NODE_BORDER, MapGraph.NODE_BORDER, MapGraph.NODE_BORDER)
            };

            // Add the style to the dictionary with the corresponding key
            nodeStyleDictionary[(MapGraph.NODEKEY)i] = nodeStyle;
        }

        // Load Room Node Types
        roomNodeTypeList = GameResources.Instance.nodeTypeList;
    }

    private void OnGUI()
    {
        // If a scriptable object of type SO_NodeGraph has been selected then process
        if (currentNodeGraph != null)
        {
            // Draw Grid
            DrawBackgroundGrid(MapGraph.GRID_SMALL, MapGraph.GRID_OPACITY, Color.gray);
            DrawBackgroundGrid(MapGraph.GRID_LARGE, MapGraph.GRID_OPACITY, Color.gray);

            // Draw dragging line
            DrawDraggingLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw connect lines between nodes
            DrawNodeConnectLine();

            // Draw Room Nodes
            DrawNodes();
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

        graphOffset += dragDelta;
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
            Color.white, null, MapGraph.CONNECT_LINE_THICKNESS);
    }

    private void DrawNodeConnectLine()
    {
        foreach (SO_MapNode roomNode in currentNodeGraph.nodeList)
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
                Vector2 arrowTailPos1 = midPos - new Vector2(-direction.y, direction.x).normalized * MapGraph.CONNECT_LINE_ARROW_SIZE;
                Vector2 arrowTailPos2 = midPos + new Vector2(-direction.y, direction.x).normalized * MapGraph.CONNECT_LINE_ARROW_SIZE;
                Vector2 arrowHeadPos = midPos + direction.normalized * MapGraph.CONNECT_LINE_ARROW_SIZE;

                // Draw line
                Handles.DrawBezier(arrowHeadPos, arrowTailPos1, arrowHeadPos, arrowTailPos1, Color.white, null, MapGraph.CONNECT_LINE_THICKNESS);
                Handles.DrawBezier(arrowHeadPos, arrowTailPos2, arrowHeadPos, arrowTailPos2, Color.white, null, MapGraph.CONNECT_LINE_THICKNESS);
                Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, MapGraph.CONNECT_LINE_THICKNESS);

                GUI.changed = true;
            }
        }
    }

    private void DrawNodes()
    {
        // Loop through all room nodes and draw them
        foreach (SO_MapNode node in currentNodeGraph.nodeList)
        {
            if (node.roomNodeType.isEntrance)
            {
                if (node.isSelected)
                {
                    node.Draw(nodeStyleDictionary[MapGraph.NODEKEY.EntranceOn]);
                }
                else
                {
                    node.Draw(nodeStyleDictionary[MapGraph.NODEKEY.Entrance]);
                }
            }
            else if (node.roomNodeType.isBossRoom)
            {
                if (node.isSelected)
                {
                    node.Draw(nodeStyleDictionary[MapGraph.NODEKEY.BossOn]);
                }
                else
                {
                    node.Draw(nodeStyleDictionary[MapGraph.NODEKEY.Boss]);
                }
            }
            else
            {
                if (node.isSelected)
                {
                    node.Draw(nodeStyleDictionary[MapGraph.NODEKEY.DefaultOn]);
                }
                else
                {
                    node.Draw(nodeStyleDictionary[MapGraph.NODEKEY.Default]);
                }
            }
        }

        GUI.changed = true;
    }

    //===========================================================================
    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        dragDelta = Vector2.zero;

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
                    SO_MapNode roomNode = GetNodeAtMouse(e);
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
                dragDelta = e.delta * 0.5f;

                // Move all nodes in graph to be in sync with grid movement
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
    private SO_MapNode GetNodeAtMouse(Event currentEvent)
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
        foreach (SO_MapNode roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected == false)
                continue;

            roomNode.isSelected = false;
            GUI.changed = true;
        }
    }

    private void SelectAllRoomNodes()
    {
        foreach (SO_MapNode roomNode in currentNodeGraph.nodeList)
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

    private void CreateNode(object mousePositionObject, SO_MapNodeType nodeType)
    {
        Vector2 mousePos = (Vector2)mousePositionObject;

        // create room node scriptable object
        SO_MapNode node = CreateInstance<SO_MapNode>();

        // add room node to current room node graph room node list
        currentNodeGraph.nodeList.Add(node);

        // set room node values
        node.Initialize(new Rect(mousePos, new Vector2(MapGraph.NODE_WIDTH, MapGraph.NODE_HEIGHT)), currentNodeGraph, nodeType);

        // add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(node, currentNodeGraph);
        AssetDatabase.SaveAssets();

        // Refresh graph node dictionary
        currentNodeGraph.OnValidate();
    }

    // Delete selected room nodes and links
    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (SO_MapNode roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected == false || roomNode.roomNodeIDList_Child.Count == 0)
                continue;

            for (int i = roomNode.roomNodeIDList_Child.Count - 1; i >= 0; i--)
            {
                // Get child room node
                SO_MapNode childRoomNode = currentNodeGraph.GetRoomNode(roomNode.roomNodeIDList_Child[i]);
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
        Queue<SO_MapNode> roomNodeDeletionQueue = new Queue<SO_MapNode>();

        foreach (SO_MapNode roomNode in currentNodeGraph.nodeList)
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
                    SO_MapNode childRoomNode = currentNodeGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode != null)
                        childRoomNode.RemoveRoomNodeID_Parent(roomNode.guid);
                }

                // iterate through child room nodes guid
                foreach (string parentRoomNodeID in roomNode.roomNodeIDList_Parent)
                {
                    // Retrive child room node
                    SO_MapNode parentRoomNode = currentNodeGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                        parentRoomNode.RemoveRoomNodeID_Child(roomNode.guid);
                }
            }
        }

        while (roomNodeDeletionQueue.Count != 0)
        {
            // Get room node from queue
            SO_MapNode roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

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
        SO_MapNodeGraph roomNodeGraph = Selection.activeObject as SO_MapNodeGraph;
        if (roomNodeGraph == null)
            return;

        currentNodeGraph = roomNodeGraph;
        GUI.changed = true;
    }
}