
[System.Serializable]
public class RoomEnemySpawnParameters
{
    public SO_DungeonLevel dungeonLevel;

    public int minTotalEnemiesToSpawn;
    public int maxTotalEnemiesToSpawn;
    public int minConcurrentEnemies;
    public int maxConcurrentEnemies;
    public int minSpawnInterval;
    public int maxSpawnInterval;
}