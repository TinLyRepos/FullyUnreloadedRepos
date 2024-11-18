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

    public event EventHandler OnInput_OffHandAction;
    private static string Action_OffHandAction = "OffHandAction";

    public event EventHandler<bool> OnInput_Run;
    private static string Action_Run = "Run";

    public event EventHandler OnInput_Jump;
    private static string Action_Jump = "Jump";

    public event EventHandler OnInput_Interact;
    private static string Action_Interact = "Interact";

    public event EventHandler OnInput_Inventory;
    public static string Action_Inventory = "Inventory";

    public event EventHandler OnInput_Quickslot1;
    private static string Action_Quickslot1 = "Quickslot1";

    public event EventHandler OnInput_Quickslot2;
    private static string Action_Quickslot2 = "Quickslot2";

    public event EventHandler OnInput_Quickslot3;
    private static string Action_Quickslot3 = "Quickslot3";

    public event EventHandler OnInput_Quickslot4;
    private static string Action_Quickslot4 = "Quickslot4";

    public event EventHandler OnInput_Quickslot5;
    private static string Action_Quickslot5 = "Quickslot5";

    public event EventHandler OnInput_Quickslot6;
    private static string Action_Quickslot6 = "Quickslot6";

    public event EventHandler OnInput_Quickslot7;
    private static string Action_Quickslot7 = "Quickslot7";

    public event EventHandler OnInput_Quickslot8;
    private static string Action_Quickslot8 = "Quickslot8";

    public event EventHandler OnInput_Quickslot9;
    private static string Action_Quickslot9 = "Quickslot9";

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

        // OFF HAND ACTION
        if (playerInput.actions[Action_OffHandAction].triggered)
            OnInput_OffHandAction?.Invoke(this, EventArgs.Empty);

        // RUN
        bool runInput = playerInput.actions[Action_Run].IsInProgress();
        OnInput_Run?.Invoke(this, runInput);

        // JUMP
        if (playerInput.actions[Action_Jump].triggered)
            OnInput_Jump?.Invoke(this, EventArgs.Empty);

        // INTERACT
        if (playerInput.actions[Action_Interact].triggered)
            OnInput_Interact?.Invoke(this, EventArgs.Empty);

        // INVENTORY
        if (playerInput.actions[Action_Inventory].triggered)
            OnInput_Inventory?.Invoke(this, EventArgs.Empty);

        // QUICKSLOTs
        if (playerInput.actions[Action_Quickslot1].triggered)
            OnInput_Quickslot1?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot2].triggered)
            OnInput_Quickslot2?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot3].triggered)
            OnInput_Quickslot3?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot4].triggered)
            OnInput_Quickslot4?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot5].triggered)
            OnInput_Quickslot5?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot6].triggered)
            OnInput_Quickslot6?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot7].triggered)
            OnInput_Quickslot7?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot8].triggered)
            OnInput_Quickslot8?.Invoke(this, EventArgs.Empty);

        if (playerInput.actions[Action_Quickslot9].triggered)
            OnInput_Quickslot9?.Invoke(this, EventArgs.Empty);
    }
}