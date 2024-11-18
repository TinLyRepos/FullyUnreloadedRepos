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

    private BoxCollider2D boxCollider2D;

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

        SealUnusedDoorway();

        DisableCollisionTilemapRenderer();
    }

    //===========================================================================
    private void SealDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPos = doorway.doorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; ++xPos)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; ++yPos)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix =
                    tilemap.GetTransformMatrix(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0));

                // Copy this tile and paste to the next tile
                tilemap.SetTile(new Vector3Int(startPos.x + 1 + xPos, startPos.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPos.x + 1 + xPos, startPos.y - yPos, 0), transformMatrix);
            }
        }
    }

    private void SealDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPos = doorway.doorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; ++yPos)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; ++xPos)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix =
                    tilemap.GetTransformMatrix(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0));

                // Copy this tile and paste to the next tile
                tilemap.SetTile(new Vector3Int(startPos.x + xPos, startPos.y - 1 - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPos.x + xPos, startPos.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPos.x + 1 + xPos, startPos.y - yPos, 0), transformMatrix);
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

    private void SealUnusedDoorway()
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
}