using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Header("DUNGEON LEVEL LIST")]
    [SerializeField] private List<SO_DungeonLevel> dungeonLevelList = default;
    [SerializeField] private int currentLevelIndex = default;

    private Room currentRoom = default;
    private Room prevRoom = default;
    private Player player = default;
    private SO_PlayerData playerData = default;

    private GameState gameState = default;

    public Room CurrentRoom => currentRoom;
    public Player Player => player;

    //===========================================================================
    protected override void Awake()
    {
        base.Awake();

        playerData = GameResources.Instance.currentPlayerData.playerData;

        // Create player gameobject
        InstantiatePlayer();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEvents_OnRoomChanged;
    }

    private void Start()
    {
        gameState = GameState.GameStarted;
    }

    private void Update()
    {
        HandleGameState();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Debug: Key R Pressed");
            gameState = GameState.GameStarted;
        }
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEvents_OnRoomChanged;
    }

    //===========================================================================
    private void StaticEvents_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    //===========================================================================
    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.GameStarted:
                PlayDungeonLevel(currentLevelIndex);
                gameState = GameState.PlayingLevel;
                break;
            case GameState.PlayingLevel:
                break;
            case GameState.EngagingEnemy:
                break;
            case GameState.BossState:
                break;
            case GameState.EngagingBoss:
                break;
            case GameState.LevelCompleted:
                break;
            case GameState.GameWon:
                break;
            case GameState.GameLost:
                break;
            case GameState.GamePaused:
                break;
            case GameState.DungeonOverviewMap:
                break;
            case GameState.RestartGame:
                break;
            default:
                break;
        }
    }

    private void InstantiatePlayer()
    {
        // Instantiate Player
        GameObject playerGameObject = Instantiate(playerData.Prefab, null);

        // Initialize Player
        player = playerGameObject.GetComponent<Player>();
        player.Initialize(playerData);
    }

    public void SetCurrentRoom(Room room)
    {
        prevRoom = currentRoom;
        currentRoom = room;

        //Debug
        //Debug.Log(room.Prefab.name)
    }

    private void PlayDungeonLevel(int levelIndex)
    {
        // Building Dungeon for Level
        bool buildSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[currentLevelIndex]);
        if (buildSuccessfully == false)
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");

        // Call Static event that room has changed
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Calculate pos of center of the room
        Vector3 roomCenter = new Vector3(
            (currentRoom.lowerBounds.x + currentRoom.upperBounds.x) * 0.5f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) * 0.5f);

        // Set player position to the closest spawn point from roomCenter
        player.gameObject.transform.position = HelperUtilities.GetClosetSpawnPosition(roomCenter);
    }

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}