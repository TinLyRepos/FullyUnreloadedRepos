using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : SingletonMonobehaviour<DungeonMap>
{
    [Header("GameObject References")]
    [SerializeField] private GameObject minimapUI;
    private Camera dungeonMapCamera;
    private Camera cameraMain;

    //===========================================================================
    private void Start()
    {
        // Cache main camera
        cameraMain = Camera.main;

        // get dungeonmap camera
        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        // If mouse button pressed and gamestate is dungeon overview map then get the room clicked
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameState.DungeonOverviewMap)
        {
            GetRoomClicked();
        }

        if (dungeonMapCamera.gameObject.activeInHierarchy)
            dungeonMapCamera.transform.position = GameManager.Instance.Player.transform.position;
    }

    //===========================================================================
    /// Get the room clicked on the map
    private void GetRoomClicked()
    {
        // Convert screen position to world position
        Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        // Check for collisions at cursor position
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        // Check if any of the colliders are a room
        foreach (Collider2D collider2D in collider2DArray)
        {
            if (collider2D.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

                // If clicked room is clear of enemies and previously visited then move player to the room
                if (instantiatedRoom.room.isCleared && instantiatedRoom.room.isVisited)
                {
                    // Move player to room
                    StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }
    }

    /// Move the player to the selected room
    private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
    {
        // Call room changed event
        StaticEventHandler.CallRoomChangedEvent(room);

        // Fade out screen to black immediately
        yield return StartCoroutine(GameManager.Instance.Fade(0f, 1f, 0f, Color.black));

        // Clear dungeon overview
        ClearDungeonOverViewMap();

        // Disable player during the fade
        GameManager.Instance.Player.playerControl.DisablePlayer();

        // Get nearest spawn point in room nearest to player
        Vector3 spawnPosition = HelperUtilities.GetClosetSpawnPosition(worldPosition);

        // Move player to new location - spawning them at the closest spawn point
        GameManager.Instance.Player.transform.position = spawnPosition;

        // Fade the screen back in
        yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

        // Enable player
        GameManager.Instance.Player.playerControl.EnablePlayer();
    }

    /// Display dungeon overview map UI
    public void DisplayDungeonOverViewMap()
    {
        // Set game state
        GameManager.Instance.prevGameState = GameManager.Instance.gameState;
        GameManager.Instance.gameState = GameState.DungeonOverviewMap;

        // Disable player
        GameManager.Instance.Player.playerControl.DisablePlayer();

        // Disable main camera and enable dungeon overview camera
        cameraMain.gameObject.SetActive(false);
        dungeonMapCamera.gameObject.SetActive(true);

        // Ensure all rooms are active so they can be displayed
        ActivateRoomsForDisplay();

        // Disable Small Minimap UI
        minimapUI.SetActive(false);
    }

    /// Clear the dungeon overview map UI
    public void ClearDungeonOverViewMap()
    {
        // Set game state
        GameManager.Instance.gameState = GameManager.Instance.prevGameState;
        GameManager.Instance.prevGameState = GameState.DungeonOverviewMap;

        // Enable player
        GameManager.Instance.Player.playerControl.EnablePlayer();

        // Enable main camera and disable dungeon overview camera
        cameraMain.gameObject.SetActive(true);
        dungeonMapCamera.gameObject.SetActive(false);

        // Enable Small Minimap UI
        minimapUI.SetActive(true);
    }

    /// Ensure all rooms are active so they can be displayed
    private void ActivateRoomsForDisplay()
    {
        // Iterate through dungeon rooms
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.RoomDictionary)
        {
            Room room = keyValuePair.Value;
            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}