using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveItem : MonoBehaviour
{
    [Header("SOUND EFFECT")]
    [SerializeField] private SoundEffectSO moveSoundEffect;
    [HideInInspector] public BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private InstantiatedRoom instantiatedRoom;
    private Vector3 previousPosition;

    //===========================================================================
    private void OnCollisionStay2D(Collision2D collision)
    {
        /// Update the obstacle positions when something comes into contact
        UpdateObstacles();
    }

    //===========================================================================
    private void Awake()
    {
        // Get component references
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();

        // Add this item to item obstacles array
        instantiatedRoom.moveableItemsList.Add(this);
    }

    //===========================================================================
    /// Update the obstacle positions
    private void UpdateObstacles()
    {
        // Make sure the item stays within the room
        ConfineItemToRoomBounds();

        // Update moveable items in obstacles array
        instantiatedRoom.UpdateMoveableObstacles();

        // capture new position post collision
        previousPosition = transform.position;

        // Play sound if moving (allowing for small velocities)
        if (Mathf.Abs(rigidBody2D.linearVelocity.x) > 0.001f ||
            Mathf.Abs(rigidBody2D.linearVelocity.y) > 0.001f)
        {
            // Play moving sound every 10 frames
            if (moveSoundEffect != null && Time.frameCount % 10 == 0)
                SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
        }
    }

    /// Confine the item to stay within the room bounds
    private void ConfineItemToRoomBounds()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = instantiatedRoom.roomColliderBounds;

        // If the item is being pushed beyond the room bounds
        // then set the item position to its previous position
        if (itemBounds.min.x <= roomBounds.min.x ||
            itemBounds.max.x >= roomBounds.max.x ||
            itemBounds.min.y <= roomBounds.min.y ||
            itemBounds.max.y >= roomBounds.max.y)
        {
            transform.position = previousPosition;
        }
    }
}