
public enum Orientation
{
    North,
    East,
    South,
    West,
    None,
}

public enum AnimationType
{
    IdleDown,
    IdleDownSide,
    IdleUp,
    IdleUpSide,
    MoveDown,
    MoveDownSide,
    MoveUp,
    MoveUpSide,
    RollDown,
    RollDownSide,
    RollUp,
    RollUpSide,
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