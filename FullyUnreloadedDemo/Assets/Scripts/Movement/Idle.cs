using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rd2D = default;
    private IdleEvent idleEvent = default;

    //===========================================================================
    private void Awake()
    {
        rd2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    //===========================================================================
    private void IdleEvent_OnIdle(IdleEvent obj)
    {
        MoveRigidBody();
    }

    //===========================================================================
    private void MoveRigidBody()
    {
        rd2D.linearVelocity = Vector2.zero;
    }
}