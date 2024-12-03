using NUnit.Framework;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;
    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    //===========================================================================
    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    //===========================================================================
    /// Process a change in room
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // Update music for room
        Assert.IsNotNull(currentRoom);
        Assert.IsNotNull(currentRoom.ambientMusic);
        MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);

        // if the room is a corridor or the entrance then return
        if (currentRoom.roomNodeType.isCorridor_Horizontal ||
            currentRoom.roomNodeType.isCorridor_Vertical ||
            currentRoom.roomNodeType.isEntrance)
            return;

        // if the room has already been defeated then return
        if (currentRoom.isCleared) return;

        // Get random number of enemies to spawn
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        // Get room enemy spawn parameters
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        // If no enemies to spawn return
        if (enemiesToSpawn == 0)
        {
            // Mark the room as cleared
            currentRoom.isCleared = true;

            return;
        }

        // Get concurrent number of enemies to spawn
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        // Update music for room
        MusicManager.Instance.PlayMusic(currentRoom.battleMusic, 0.2f, 0.5f);

        // Lock doors
        currentRoom.instantiatedRoom.LockDoors();

        // Spawn enemies
        SpawnEnemies();
    }

    //===========================================================================
    /// Spawn the enemies
    private void SpawnEnemies()
    {
        // Set gamestate engaging boss
        if (GameManager.Instance.gameState == GameState.BossState)
        {
            GameManager.Instance.prevGameState = GameState.BossState;
            GameManager.Instance.gameState = GameState.EngagingBoss;
        }

        // Set gamestate engaging enemies
        else if (GameManager.Instance.gameState == GameState.PlayingLevel)
        {
            GameManager.Instance.prevGameState = GameState.PlayingLevel;
            GameManager.Instance.gameState = GameState.EngagingEnemy;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    /// Spawn the enemies coroutine
    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        // Create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        // Check we have somewhere to spawn the enemies
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            // Loop through to create all the enemeies
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // wait until current enemy count is less than max concurrent enemies
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                // Create Enemy - Get next enemy type to spawn 
                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    /// Get a random spawn interval between the minimum and maximum values
    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }

    /// Get a random number of concurrent enemies between the minimum and maximum values
    private int GetConcurrentEnemies()
    {
        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

    /// Create an enemy in the specified position
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        // keep track of the number of enemies spawned so far 
        enemiesSpawnedSoFar++;

        // Add one to the current enemy count - this is reduced when an enemy is destroyed
        currentEnemyCount++;

        // Get current dungeon level
        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // Instantiate enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        // Initialize Enemy
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        // subscribe to enemy destroyed event
        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
    }

    /// Process enemy destroyed
    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        // Unsubscribe from event
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        // reduce current enemy count
        currentEnemyCount--;

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            currentRoom.isCleared = true;

            // Set game state
            if (GameManager.Instance.gameState == GameState.EngagingEnemy)
            {
                GameManager.Instance.gameState = GameState.PlayingLevel;
                GameManager.Instance.prevGameState = GameState.EngagingEnemy;
            }
            else if (GameManager.Instance.gameState == GameState.EngagingBoss)
            {
                GameManager.Instance.gameState = GameState.BossState;
                GameManager.Instance.prevGameState = GameState.EngagingBoss;
            }

            // unlock doors
            currentRoom.instantiatedRoom.UnlockDoors(Settings.DOOR_UNLOCK_DELAY);

            // Update music for room
            MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);

            // Trigger room enemies defeated event
            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }
}