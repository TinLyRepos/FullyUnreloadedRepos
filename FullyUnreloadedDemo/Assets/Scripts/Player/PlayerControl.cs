using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private SO_MovementData movementData = default;

    private Player player;
    private float moveSpeed;

    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private bool isPlayerRolling = false;
    private float playerRollCDTimer = 0.0f;

    // Weapon
    private int currentWeaponIndex = 1;

    //===========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }

    //===========================================================================
    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementData.GetMoveSpeed();
    }

    private void Start()
    {
        HelperUtilities.CacheMainCamera();

        SetStartingWeapon();

        SetPlayerAnimationSpeed();
    }

    private void Update()
    {
        if (isPlayerRolling)
            return;

        MovementInput();
        WeaponInput();

        PlayerRollCooldownTimer();
    }

    //===========================================================================
    private void SetPlayerAnimationSpeed()
    {
        // Set animator speed to match movement speed
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void MovementInput()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);
        direction.Normalize();

        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            else if (playerRollCDTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    private void WeaponInput()
    {
        Vector3 weaponDirection = Vector3.zero;
        float weaponAngle = default;
        float playerAngle = default;
        AimDirection playerAimDirection;

        // Aim Weapon input
        AimWeaponInput(out weaponDirection, out weaponAngle, out playerAngle, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngle, out float playerAngle, out AimDirection playerAimDirection)
    {
        // Get Mouse to world position
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseToWorldPosition();

        // Calculate direction vector of mouse cursor from weapon shoot position
        weaponDirection = mouseWorldPosition - player.activeWeapon.GetShootPosition();

        // Get Weapon to cursor angle
        weaponAngle = HelperUtilities.GetAngleFromVector(weaponDirection);

        // Calculate direction vector of mouse cursor from player transform position
        Vector3 playerDirection = mouseWorldPosition - transform.position;

        // Get Player to cursor angle
        playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);

        // Set player aim direction
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngle);

        // Trigger weapon aim event
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngle, weaponAngle, weaponDirection);
    }

    private void PlayerRollCooldownTimer()
    {
        if (playerRollCDTimer >= 0f)
            playerRollCDTimer -= Time.deltaTime;
    }

    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        // minDistance used to decide when to exit coroutine loop
        float minDistance = 0.2f;

        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + direction * movementData.rollDistance;

        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, 
                player.transform.position, movementData.rollSpeed, direction, isPlayerRolling);
            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;

        // Set cooldown timer
        playerRollCDTimer = movementData.rollCDTime;

        player.transform.position = targetPosition;
    }

    private void SetStartingWeapon()
    {
        int weaponIndex = 1;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon.weaponDetails == player.playerData.StartingWeapon)
            {
                // Set Weapon By Index
                if (weaponIndex - 1 < player.weaponList.Count)
                {
                    currentWeaponIndex = weaponIndex;
                    player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
                }
                break;
            }
            weaponIndex++;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementData), movementData);
    }
#endif
}