
public enum Orientation
{
    North,
    East,
    South,
    West,
    None,
}

public enum AimDirection
{
    Up,
    UpRight,
    UpLeft,
    Down,
    Left,
    Right,
}

public enum GameState
{
    GameStarted,
    PlayingLevel,
    EngagingEnemy,
    BossState,
    EngagingBoss,
    LevelCompleted,
    GameWon,
    GameLost,
    GamePaused,
    DungeonOverviewMap,
    RestartGame,
}

public enum ChestSpawnEvent
{
    onRoomEntry,
    onEnemiesDefeated
}

public enum ChestSpawnPosition
{
    atSpawnerPosition,
    atPlayerPosition
}

public enum ChestState
{
    closed,
    healthItem,
    ammoItem,
    weaponItem,
    empty
}