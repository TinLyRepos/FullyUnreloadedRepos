using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    // 2D array to store movement penalty from tilemaps to be used in A* pathfinding
    [HideInInspector] public int[,] aStarMovementPenalty;
    // 2D array to store position of moveable items that are obstacles
    [HideInInspector] public int[,] aStarItemObstacles;

    [HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>();

    [Header("OBJECT REFERENCES")]
    [SerializeField] private GameObject environmentGameObject;

    private BoxCollider2D boxCollider2D;

    //===========================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player triggered the collider
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.CurrentRoom)
        {
            // Set room as visited
            this.room.isVisited = true;

            // Call room changed event
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    //===========================================================================
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Save room collider bounds
        roomColliderBounds = boxCollider2D.bounds;
    }

    //===========================================================================
    public void Initialize(GameObject roomGameObject)
    {
        PopulateTilemapMemberVariables(roomGameObject);

        BlockUnusedDoorway();

        AddObstaclesAndPreferredPaths();

        CreateItemObstaclesArray();

        AddDoorsToRoom();

        DisableCollisionTilemapRenderer();
    }

    //===========================================================================
    private void SealDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPos = doorway.doorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(
                    new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0));

                // Copy tile
                tilemap.SetTile(new Vector3Int(startPos.x + 1 + xPos, startPos.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(
                    new Vector3Int(startPos.x + 1 + xPos, startPos.y - yPos, 0), transformMatrix);
            }
        }
    }

    private void SealDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPos = doorway.doorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(
                    new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0));

                // Copy tile
                tilemap.SetTile(new Vector3Int(startPos.x + xPos, startPos.y - 1 - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(
                    new Vector3Int(startPos.x + xPos, startPos.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    private void SealDoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.North:
            case Orientation.South:
                SealDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.East:
            case Orientation.West:
                SealDoorwayVertically(tilemap, doorway);
                break;
            default:
                break;
        }
    }

    //===========================================================================
    private void AddObstaclesAndPreferredPaths()
    {
        // this array will be populated with wall obstacles 
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        // Loop thorugh all grid squares
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // Set default movement penalty for grid sqaures
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                // Add obstacles for collision tiles the enemy can't walk on
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                // Add preferred path for enemies (1 is the preferred path value, default value for
                // a grid location is specified in the Settings).
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    private void CreateItemObstaclesArray()
    {
        // this array will be populated during gameplay with any moveable obstacles
        aStarItemObstacles = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
    }

    //===========================================================================
    private void BlockUnusedDoorway()
    {
        // Go through all doorway
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected)
                continue;

            // Block unconnected doorway using tiles on tilemaps
            if (collisionTilemap != null)
                SealDoorwayOnTilemapLayer(collisionTilemap, doorway);

            if (minimapTilemap != null)
                SealDoorwayOnTilemapLayer(minimapTilemap, doorway);

            if (groundTilemap != null)
                SealDoorwayOnTilemapLayer(groundTilemap, doorway);

            if (decoration1Tilemap != null)
                SealDoorwayOnTilemapLayer(decoration1Tilemap, doorway);

            if (decoration2Tilemap != null)
                SealDoorwayOnTilemapLayer(decoration2Tilemap, doorway);

            if (frontTilemap != null)
                SealDoorwayOnTilemapLayer(frontTilemap, doorway);
        }
    }

    private void AddDoorsToRoom()
    {
        // if the room is a corridor then return
        if (room.roomNodeType.isCorridor_Horizontal || room.roomNodeType.isCorridor_Vertical)
            return;

        // Instantiate door prefabs at doorway positions
        foreach (Doorway doorway in room.doorwayList)
        {

            // if the doorway prefab isn't null and the doorway is connected
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.North)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = 
                        new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.South)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = 
                        new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.East)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = 
                        new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.West)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = 
                        new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }

                // Get door component
                Door doorComponent = door.GetComponent<Door>();

                // Set if door is part of a boss room
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    // lock the door to prevent access to the room
                    doorComponent.LockDoor();

                    // Instantiate skull icon for minimap by door
                    // GameObject skullIcon = Instantiate(GameResources.Instance.minimapSkullPrefab, gameObject.transform);
                    // skullIcon.transform.localPosition = door.transform.localPosition;
                }
            }
        }
    }

    //===========================================================================
    private void PopulateTilemapMemberVariables(GameObject roomGameObject)
    {
        // Get grid component
        grid = roomGameObject.GetComponentInChildren<Grid>();

        // Get tilemaps in children
        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.CompareTag("groundTilemap"))
                groundTilemap = tilemap;
            else if (tilemap.gameObject.CompareTag("decoration1Tilemap"))
                decoration1Tilemap = tilemap;
            else if (tilemap.gameObject.CompareTag("decoration2Tilemap"))
                decoration2Tilemap = tilemap;
            else if (tilemap.gameObject.CompareTag("frontTilemap"))
                frontTilemap = tilemap;
            else if (tilemap.gameObject.CompareTag("collisionTilemap"))
                collisionTilemap = tilemap;
            else if (tilemap.gameObject.CompareTag("minimapTilemap"))
                minimapTilemap = tilemap;
        }
    }

    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0f)
            yield return new WaitForSeconds(doorUnlockDelay);

        Door[] doorArray = GetComponentsInChildren<Door>();

        // Trigger open doors
        foreach (Door door in doorArray)
        {
            door.UnlockDoor();
        }

        // Enable room trigger collider
        boxCollider2D.enabled = true;
    }

    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // Set default movement penalty for grid sqaures
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    public void ActivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(true);
    }

    public void DeactivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
            environmentGameObject.SetActive(false);
    }

    //===========================================================================
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        // Trigger lock doors
        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        // Disable room trigger collider
        boxCollider2D.enabled = false;
    }

    public void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            Vector3Int colliderBoundsMin = grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            Vector3Int colliderBoundsMax = grid.WorldToCell(moveItem.boxCollider2D.bounds.max);

            // Loop through and add moveable item collider bounds to obstacle array
            for (int i = colliderBoundsMin.x; i <= colliderBoundsMax.x; i++)
            {
                for (int j = colliderBoundsMin.y; j <= colliderBoundsMax.y; j++)
                {
                    aStarItemObstacles[i - room.templateLowerBounds.x, j - room.templateLowerBounds.y] = 0;
                }
            }
        }
    }
}