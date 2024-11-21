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

    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParameters> roomLevelEnemySpawnParametersList;

    public string roomID_parent = default;
    public List<string> roomIDList_Child = default;
    public List<Doorway> doorwayList = default;
    public InstantiatedRoom instantiatedRoom = default;

    public bool isPositioned = default;
    public bool isLit = default;
    public bool isCleared = default;
    public bool isVisited = default;

    public MusicTrackSO battleMusic;
    public MusicTrackSO ambientMusic;

    public Room()
    {
        roomIDList_Child = new List<string>();
        doorwayList = new List<Doorway>();
    }

    /// Get the number of enemies to spawn for this room in this dungeon level
    public int GetNumberOfEnemiesToSpawn(SO_DungeonLevel dungeonLevel)
    {
        foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomLevelEnemySpawnParametersList)
        {
            if (roomEnemySpawnParameters.dungeonLevel == dungeonLevel)
            {
                return Random.Range(roomEnemySpawnParameters.minTotalEnemiesToSpawn, roomEnemySpawnParameters.maxTotalEnemiesToSpawn);
            }
        }
        return 0;
    }

    /// Get the room enemy spawn parameters for this dungeon level - if none found then return null
    public RoomEnemySpawnParameters GetRoomEnemySpawnParameters(SO_DungeonLevel dungeonLevel)
    {
        foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomLevelEnemySpawnParametersList)
        {
            if (roomEnemySpawnParameters.dungeonLevel == dungeonLevel)
            {
                return roomEnemySpawnParameters;
            }
        }
        return null;
    }
}