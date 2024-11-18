using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string id = string.Empty;

    public string templateID = default;
    public GameObject prefab = default;
    public SO_NodeType roomNodeType = default;

    public Vector2Int lowerBounds = Vector2Int.zero;
    public Vector2Int upperBounds = Vector2Int.zero;
    public Vector2Int templateLowerBounds = Vector2Int.zero;
    public Vector2Int templateUpperBounds = Vector2Int.zero;
    public Vector2Int[] spawnPositionArray = null;

    public string roomID_parent = default;
    public List<string> roomIDList_Child = default;
    public List<Doorway> doorwayList = default;
    public InstantiatedRoom instantiatedRoom = default;

    public bool isPositioned = default;
    public bool isLit = default;
    public bool isCleared = default;
    public bool isVisited = default;

    public Room()
    {
        roomIDList_Child = new List<string>();
        doorwayList = new List<Doorway>();
    }
}