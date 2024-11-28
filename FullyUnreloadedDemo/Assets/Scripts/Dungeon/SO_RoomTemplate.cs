using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Dungeon Room")]
public class SO_RoomTemplate : ScriptableObject
{
    [HideInInspector] public string guid = string.Empty;

    [Header("ROOM PREFAB")]
    public GameObject prefab;
    private GameObject prefab_cache;
    [Space]
    [Header("ROOM CONFIG")]
    public SO_MapNodeType roomNodeType;
    public Vector2Int lowerBounds; // Use by Doorway.cs to seal unused door - bottom left
    public Vector2Int upperBounds; // Use by Doorway.cs to seal unused door - top right
    public List<Doorway> doorwayList;
    public Vector2Int[] spawnPositionArray;
    [Space]
    [Header("ROOM MUSIC")]
    public MusicTrackSO battleMusic;
    public MusicTrackSO ambientMusic;
    [Space]
    [Header("ENEMY DETAILS")]
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;

    //===========================================================================
    public List<Doorway> GetDoorwayList() { return doorwayList; }

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
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ambientMusic), ambientMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        // Check enemies and room spawn parameters for levels
        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);

            foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomEnemySpawnParametersList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(roomEnemySpawnParameters.dungeonLevel), roomEnemySpawnParameters.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minTotalEnemiesToSpawn), roomEnemySpawnParameters.minTotalEnemiesToSpawn, nameof(roomEnemySpawnParameters.maxTotalEnemiesToSpawn), roomEnemySpawnParameters.maxTotalEnemiesToSpawn, true);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minSpawnInterval), roomEnemySpawnParameters.minSpawnInterval, nameof(roomEnemySpawnParameters.maxSpawnInterval), roomEnemySpawnParameters.maxSpawnInterval, true);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minConcurrentEnemies), roomEnemySpawnParameters.minConcurrentEnemies, nameof(roomEnemySpawnParameters.maxConcurrentEnemies), roomEnemySpawnParameters.maxConcurrentEnemies, false);

                bool isEnemyTypesListForDungeonLevel = false;

                // Validate enemy types list
                foreach (SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectsByLevel in enemiesByLevelList)
                {
                    if (dungeonObjectsByLevel.dungeonLevel == roomEnemySpawnParameters.dungeonLevel && dungeonObjectsByLevel.spawnableObjectRatioList.Count > 0)
                        isEnemyTypesListForDungeonLevel = true;

                    HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectsByLevel.dungeonLevel), dungeonObjectsByLevel.dungeonLevel);

                    foreach (SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectsByLevel.spawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.dungeonObject), dungeonObjectRatio.dungeonObject);

                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.ratio), dungeonObjectRatio.ratio, false);
                    }

                }

                if (isEnemyTypesListForDungeonLevel == false && roomEnemySpawnParameters.dungeonLevel != null)
                {
                    Debug.Log("No enemy types specified in for dungeon level " + roomEnemySpawnParameters.dungeonLevel.Name + " in gameobject " + this.name.ToString());
                }
            }
        }

        // Check spawn positions populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }
#endif
}