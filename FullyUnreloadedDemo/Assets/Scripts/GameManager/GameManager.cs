using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Header("DUNGEON LEVEL LIST")]
    [SerializeField] private List<SO_DungeonLevel> dungeonLevelList = default;
    [SerializeField] private int currentLevelIndex = default;

    private GameState gameState = default;

    //===========================================================================
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

    //===========================================================================
    private void PlayDungeonLevel(int levelIndex)
    {
        // Building Dungeon for Level
        bool buildSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[currentLevelIndex]);
        if (buildSuccessfully == false)
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
    }

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtils.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}