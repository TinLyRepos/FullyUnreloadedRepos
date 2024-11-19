using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Dungeon Room")]
public class SO_RoomTemplate : ScriptableObject
{
    public string guid = string.Empty;

    [Header("ROOM PREFAB")]
    public GameObject prefab;
    private GameObject prefab_cache;
    [Space]
    [Header("ROOM CONFIG")]
    public SO_NodeType roomNodeType;
    public Vector2Int lowerBounds; // Use by Doorway.cs to seal unused door - bottom left
    public Vector2Int upperBounds; // Use by Doorway.cs to seal unused door - top right
    [Space]
    [Header("ENVIRONMENT CONFIG")]
    public List<Doorway> doorwayList;
    public Vector2Int[] spawnPositionArray;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Set unique GUID if empty or the prefab changes
        if (guid == "" || prefab_cache != prefab)
        {
            guid = GUID.Generate().ToString();
            prefab_cache = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        // Check spawn positions populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }
#endif
}