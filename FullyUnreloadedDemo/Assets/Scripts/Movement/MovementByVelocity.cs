using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private MovementByVelocityEvent movementByVelocityEvent;

    //===========================================================================
    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    //===========================================================================
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent arg1, MovementByVelocityArgs arg2)
    {
        MoveRigidBody(arg2.moveDirection, arg2.moveSpeed);
    }

    //===========================================================================
    private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        rigidBody2D.linearVelocity = moveDirection * moveSpeed;
    }
}