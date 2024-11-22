using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeGraph", menuName = "Scriptable Objects/Map Editor/Node Graph")]
public class SO_MapNodeGraph : ScriptableObject
{
    [HideInInspector] public SO_NodeTypeList nodeTypeList = default;
    [HideInInspector] public List<SO_Node> nodeList = new List<SO_Node>();
    [HideInInspector] public Dictionary<string, SO_Node> nodeDictionary = new Dictionary<string, SO_Node>();

    //===========================================================================
    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    //===========================================================================
    private void LoadRoomNodeDictionary()
    {
        nodeDictionary.Clear();

        foreach (SO_Node roomNode in nodeList)
            nodeDictionary[roomNode.guid] = roomNode;
    }

    //===========================================================================
    public SO_Node GetRoomNode(string roomNodeID)
    {
        if (nodeDictionary.TryGetValue(roomNodeID, out SO_Node roomNode))
            return roomNode;

        return null;
    }

    public SO_Node GetRoomNode(SO_NodeType roomNodeType)
    {
        foreach (SO_Node roomNode in nodeList)
        {
            if (roomNode.roomNodeType == roomNodeType)
                return roomNode;
        }
        return null;
    }

    public IEnumerable<SO_Node> GetChildRoomNodes(SO_Node parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.roomNodeIDList_Child)
        {
            yield return GetRoomNode(childNodeID);
        }
    }

    //===========================================================================
#if UNITY_EDITOR
    [HideInInspector] public SO_Node roomNodeStart = null;
    [HideInInspector] public Vector2 endLinePosition = Vector2.zero;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetDrawStartNode(SO_Node node, Vector2 position)
    {
        roomNodeStart = node;
        endLinePosition = position;
    }
#endif
}