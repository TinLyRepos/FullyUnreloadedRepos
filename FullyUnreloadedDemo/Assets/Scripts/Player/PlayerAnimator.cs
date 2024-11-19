using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerAnimator : MonoBehaviour
{
    private Player player;

    //===========================================================================
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.idleEvent.OnIdle += IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    //===========================================================================
    private void IdleEvent_OnIdle(IdleEvent obj)
    {
        // Set Idle Animation Parameters
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        // Reset Aim Animation Parameters
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimDown, false);

        // Set Aim Animation Parameters
        switch (aimWeaponEventArgs.aimDirection)
        {
            case AimDirection.Up:
                player.animator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.UpRight:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.UpLeft:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.Down:
                player.animator.SetBool(Settings.aimDown, true);
                break;
            case AimDirection.Left:
                player.animator.SetBool(Settings.aimLeft, true);
                break;
            case AimDirection.Right:
                player.animator.SetBool(Settings.aimRight, true);
                break;
            default:
                break;
        }
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent arg1, MovementByVelocityArgs arg2)
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1, MovementToPositionArgs arg2)
    {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();
        SetMovementToPositionAnimationParameters(arg2);
    }

    //===========================================================================
    private void InitializeAimAnimationParameters()
    {
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimDown, false);
    }

    private void InitializeRollAnimationParameters()
    {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollLeft, false);
        player.animator.SetBool(Settings.rollUp, false);
    }

    private void SetMovementToPositionAnimationParameters(MovementToPositionArgs movementToPositionArgs)
    {
        if (movementToPositionArgs.isRolling == false)
            return;

        // Animate roll
        if (movementToPositionArgs.moveDirection.x > 0f)
        {
            player.animator.SetBool(Settings.rollRight, true);
        }
        else if (movementToPositionArgs.moveDirection.x < 0f)
        {
            player.animator.SetBool(Settings.rollLeft, true);
        }
        else if (movementToPositionArgs.moveDirection.y > 0f)
        {
            player.animator.SetBool(Settings.rollUp, true);
        }
        else if (movementToPositionArgs.moveDirection.y < 0f)
        {
            player.animator.SetBool(Settings.rollDown, true);
        }
    }
}