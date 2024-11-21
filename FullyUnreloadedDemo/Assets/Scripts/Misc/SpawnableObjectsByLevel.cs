using System.Collections.Generic;

[System.Serializable]
public class SpawnableObjectsByLevel<T>
{
    public SO_DungeonLevel dungeonLevel;
    public List<SpawnableObjectRatio<T>> spawnableObjectRatioList;
}