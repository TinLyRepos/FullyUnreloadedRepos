using De2Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Header("GAMEOBJECT REFERENCES")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("DUNGEON LEVEL LIST")]
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList = default;
    [SerializeField] private int currentDungeonLevelIndex = default;

    [Header("PLAYER")]
    [SerializeField] private SO_PlayerData playerData_TheCowgirl = default;
    private SO_PlayerData activePlayerData = default;

    private Room currentRoom = default;
    private Room prevRoom = default;

    [HideInInspector] public GameState currentGameState = default;
    [HideInInspector] public GameState previousGameState = default;

    private InstantiatedRoom bossRoom;
    private bool isFading = false;

    public Room CurrentRoom => currentRoom;

    //===========================================================================
    protected override void Awake()
    {
        base.Awake();
        De2Helper.CacheMainCamera();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEvents_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void Start()
    {
        previousGameState = GameState.GameStarted;
        currentGameState = GameState.GameStarted;

        // Create player gameobject
        activePlayerData = playerData_TheCowgirl;
        Player.Instance.Initialize(activePlayerData);

        // Set screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
    }

    private void Update()
    {
        HandleGameState();

        //if (Input.GetKeyDown(KeyCode.Home))
        //{
        //    Debug.Log("Debug: Key R Pressed");
        //    gameState = GameState.GameStarted;
        //}
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEvents_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    //===========================================================================
    private void StaticEvents_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        previousGameState = currentGameState;
        currentGameState = GameState.GameLost;
    }

    //===========================================================================
    private void HandleGameState()
    {
        switch (currentGameState)
        {
            case GameState.GameStarted:
                PlayDungeonLevel(currentDungeonLevelIndex);
                currentGameState = GameState.PlayingLevel;
                RoomEnemiesDefeated();
                break;
            case GameState.PlayingLevel:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGameMenu();
                if (Input.GetKeyDown(KeyCode.Tab))
                    DisplayDungeonOverviewMap();
                break;
            case GameState.EngagingEnemy:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGameMenu();
                break;
            case GameState.BossState:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGameMenu();
                if (Input.GetKeyDown(KeyCode.Tab))
                    DisplayDungeonOverviewMap();
                break;
            case GameState.EngagingBoss:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGameMenu();
                break;
            case GameState.LevelCompleted:
                StartCoroutine(LevelCompleted());
                break;
            case GameState.GameWon:
                if (previousGameState != GameState.GameWon)
                    StartCoroutine(GameWon());
                break;
            case GameState.GameLost:
                if (previousGameState != GameState.GameLost)
                {
                    StopAllCoroutines(); // Prevent messages if you clear the level just as you get killed
                    StartCoroutine(GameLost());
                }
                break;
            case GameState.GamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGameMenu();
                break;
            case GameState.DungeonOverviewMap:
                if (Input.GetKeyUp(KeyCode.Tab))
                    DungeonMap.Instance.ClearDungeonOverViewMap();
                break;
            case GameState.RestartGame:
                RestartGame();
                break;
            default:
                break;
        }
    }

    //===========================================================================
    private void PlayDungeonLevel(int levelIndex)
    {
        // Building Dungeon for Level
        bool buildSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[currentDungeonLevelIndex]);
        if (buildSuccessfully == false)
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");

        // Call Static event that room has changed
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Calculate pos of center of the room
        Vector3 roomCenter = new Vector3(
            (currentRoom.lowerBounds.x + currentRoom.upperBounds.x) * 0.5f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) * 0.5f);

        // Set player position to the closest spawn point from roomCenter
        Player.Instance.Position = HelperUtilities.GetClosetSpawnPosition(roomCenter);

        // Display Dungeon Level Text
        StartCoroutine(DisplayDungeonLevelText());
    }

    private void RoomEnemiesDefeated()
    {
        // Initialise dungeon as being cleared - but then test each room
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        // Loop through all dungeon rooms to see if cleared of enemies
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.RoomDictionary)
        {
            // skip boss room for time being
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            // check if other rooms have been cleared of enemies
            if (!keyValuePair.Value.isCleared)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        // Set game state
        // If dungeon level completly cleared (i.e. dungeon cleared apart from boss and there is no boss room OR dungeon cleared apart from boss and boss room is also cleared)
        if (isDungeonClearOfRegularEnemies && bossRoom == null ||
            isDungeonClearOfRegularEnemies && bossRoom.room.isCleared)
        {
            // Are there more dungeon levels then
            if (currentDungeonLevelIndex < dungeonLevelList.Count - 1)
            {
                currentGameState = GameState.LevelCompleted;
            }
            else
            {
                currentGameState = GameState.GameWon;
            }
        }
        // Else if dungeon level cleared apart from boss room
        else if (isDungeonClearOfRegularEnemies)
        {
            currentGameState = GameState.BossState;
            StartCoroutine(BossStage());
        }
    }

    private void DisplayDungeonOverviewMap()
    {
        // return if fading
        if (isFading)
            return;

        // Display dungeonOverviewMap
        DungeonMap.Instance.DisplayDungeonOverViewMap();
    }

    //===========================================================================
    /// Display the dungeon level text
    private IEnumerator DisplayDungeonLevelText()
    {
        // Set screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        // Player.playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelIndex + 1).ToString() + 
            "\n\n" + dungeonLevelList[currentDungeonLevelIndex].Name.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        // Player.playerControl.EnablePlayer();

        // Fade In
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    /// Display the message text for displaySeconds
    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        // Set text
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        // Display the message for the given time
        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        // else display the message until the return button is pressed
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        // Clear text
        messageTextTMP.SetText(string.Empty);
    }

    /// Enter boss stage
    private IEnumerator BossStage()
    {
        // Activate boss room
        bossRoom.gameObject.SetActive(true);

        // Unlock boss room
        bossRoom.UnlockDoors(0f);

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);

        // Fade in canvas to display text message
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // Display boss message
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE  " + 
            GameResources.Instance.currentPlayerData.playerName + 
            "!  YOU'VE SURVIVED ....SO FAR\n\nNOW FIND AND DEFEAT THE BOSS....GOOD LUCK!", Color.white, 5f));

        // Fade out canvas
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    /// Show level as being completed - load next level
    private IEnumerator LevelCompleted()
    {
        // Play next level
        currentGameState = GameState.PlayingLevel;

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);

        // Fade in canvas to display text message
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // Display level completed
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + 
            GameResources.Instance.currentPlayerData.playerName + 
            "! \n\nYOU'VE SURVIVED THIS DUNGEON LEVEL", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("COLLECT ANY LOOT ....THEN PRESS RETURN\n\nTO DESCEND FURTHER INTO THE DUNGEON", Color.white, 5f));

        // Fade out canvas
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // When player presses the return key proceed to the next level
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; // to avoid enter being detected twice

        // Increase index to next level
        currentDungeonLevelIndex++;
        PlayDungeonLevel(currentDungeonLevelIndex);
    }

    /// Fade Canvas Group
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        isFading = true;

        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;
        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }

        isFading = false;
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.GameWon;

        // Disable player
        // Player.playerControl.DisablePlayer();

        // Wait 1 seconds
        yield return new WaitForSeconds(1f);

        // Fade Out
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // Display game won
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + 
            GameResources.Instance.currentPlayerData.playerName +
            "! YOU HAVE DEFEATED THE DUNGEON", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        // Set game state to restart game
        currentGameState = GameState.RestartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.GameLost;

        // Disable player
        // Player.playerControl.DisablePlayer();

        // Wait 1 seconds
        yield return new WaitForSeconds(1f);

        // Fade Out
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // Disable enemies (FindObjectsOfType is resource hungry - but ok to use in this end of game situation)
        Enemy[] enemyArray = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemyArray)
            enemy.gameObject.SetActive(false);

        // Display game lost
        yield return StartCoroutine(DisplayMessageRoutine("BAD LUCK " + 
            GameResources.Instance.currentPlayerData.playerName + 
            "! YOU HAVE SUCCUMBED TO THE DUNGEON", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        // Set game state to restart game
        currentGameState = GameState.RestartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    //===========================================================================
    public void SetCurrentRoom(Room room)
    {
        prevRoom = currentRoom;
        currentRoom = room;

        //Debug
        //Debug.Log(room.Prefab.name)
    }

    public Sprite GetPlayerMinimapIcon()
    {
        return activePlayerData.MiniMapIcon;
    }

    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelIndex];
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public void PauseGameMenu()
    {
        if (currentGameState != GameState.GamePaused)
        {
            pauseMenu.SetActive(true);
            // Player.playerControl.DisablePlayer();

            // Set game state
            previousGameState = currentGameState;
            currentGameState = GameState.GamePaused;
        }
        else if (currentGameState == GameState.GamePaused)
        {
            pauseMenu.SetActive(false);
            // Player.playerControl.EnablePlayer();

            // Set game state
            currentGameState = previousGameState;
            previousGameState = GameState.GamePaused;
        }
    }

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}