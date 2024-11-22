using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeGraph", menuName = "Scriptable Objects/Map Editor/Node Graph")]
public class SO_MapNodeGraph : ScriptableObject
{
    [HideInInspector] public SO_MapNodeTypeList nodeTypeList = default;
    [HideInInspector] public List<SO_MapNode> nodeList = new List<SO_MapNode>();
    [HideInInspector] public Dictionary<string, SO_MapNode> nodeDictionary = new Dictionary<string, SO_MapNode>();

    //===========================================================================
    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    //===========================================================================
    private void LoadRoomNodeDictionary()
    {
        nodeDictionary.Clear();

        foreach (SO_MapNode roomNode in nodeList)
            nodeDictionary[roomNode.guid] = roomNode;
    }

    //===========================================================================
    public SO_MapNode GetRoomNode(string roomNodeID)
    {
        if (nodeDictionary.TryGetValue(roomNodeID, out SO_MapNode roomNode))
            return roomNode;

        return null;
    }

    public SO_MapNode GetRoomNode(SO_MapNodeType roomNodeType)
    {
        foreach (SO_MapNode roomNode in nodeList)
        {
            if (roomNode.roomNodeType == roomNodeType)
                return roomNode;
        }
        return null;
    }

    public IEnumerable<SO_MapNode> GetChildRoomNodes(SO_MapNode parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.roomNodeIDList_Child)
        {
            yield return GetRoomNode(childNodeID);
        }
    }

    //===========================================================================
#if UNITY_EDITOR
    [HideInInspector] public SO_MapNode roomNodeStart = null;
    [HideInInspector] public Vector2 endLinePosition = Vector2.zero;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetDrawStartNode(SO_MapNode node, Vector2 position)
    {
        roomNodeStart = node;
        endLinePosition = position;
    }
#endif
}