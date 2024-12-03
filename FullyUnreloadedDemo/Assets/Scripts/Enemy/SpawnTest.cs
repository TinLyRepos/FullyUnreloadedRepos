using De2Utils;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    //===========================================================================
    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();

            if (enemyDetails != null)
                instantiatedEnemyList.Add(Instantiate(enemyDetails.enemyPrefab, HelperUtilities.GetClosetSpawnPosition(De2Helper.GetMouseToWorldPosition()), Quaternion.identity));
        }
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    //===========================================================================
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // Destroy any spawned enemies
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }

        SO_RoomTemplate roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);

        if (roomTemplate != null)
        {
            testLevelSpawnList = roomTemplate.enemiesByLevelList;

            // Create RandomSpawnableObject helper class
            randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
        }
    }
}