using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CustomPlayerInputEvent : SingletonMonobehaviour<CustomPlayerInputEvent>
{
    private PlayerInput playerInput = default;

    // Action String
    public event EventHandler<Vector2> OnInput_Move;
    private static string Action_Move = "Move";

    public event EventHandler OnInput_MainHandAction;
    private static string Action_MainHandAction = "MainHandAction";

    public event EventHandler OnInput_Interact;
    private static string Action_Interact = "Interact";

    //===========================================================================
    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
    }

    //===========================================================================
    private void Update()
    {
        // MOVE
        Vector2 movementInput = playerInput.actions[Action_Move].ReadValue<Vector2>();
        OnInput_Move?.Invoke(this, movementInput);

        // MAIN HAND ACTION
        if (playerInput.actions[Action_MainHandAction].triggered)
            OnInput_MainHandAction?.Invoke(this, EventArgs.Empty);

        // INTERACT
        if (playerInput.actions[Action_Interact].triggered)
            OnInput_Interact?.Invoke(this, EventArgs.Empty);
    }
}