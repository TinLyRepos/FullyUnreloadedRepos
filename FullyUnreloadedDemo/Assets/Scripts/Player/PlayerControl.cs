using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private SO_MovementData movementData = default;

    private Player player;
    private float moveSpeed;

    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    public bool isPlayerRolling = false;
    private float playerRollCDTimer = 0.0f;

    // Weapon
    private int currentWeaponIndex = 1;
    private bool leftMouseDownPreviousFrame = false;

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
        waitForFixedUpdate = new WaitForFixedUpdate();

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

    //===========================================================================
    private void WeaponInput()
    {
        Vector3 weaponDirection = Vector3.zero;
        float weaponAngle = default;
        float playerAngle = default;
        AimDirection playerAimDirection;

        // Aim Weapon input
        AimWeaponInput(out weaponDirection, out weaponAngle, out playerAngle, out playerAimDirection);

        // Fire Weapon Input
        FireWeaponInput(weaponDirection, weaponAngle, playerAngle, playerAimDirection);

        // Switch weapon
        SwitchWeaponInput();

        // Reload Weapon Input
        ReloadWeaponInput();
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

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        // Fire when left mouse button is clicked
        if (Input.GetMouseButton(0))
        {
            // Trigger fire weapon event
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        // if current weapon is reloading return
        if (currentWeapon.isWeaponReloading)
            return;

        // remaining ammo is less than clip capacity then return and not infinite ammo then return
        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && 
            currentWeapon.weaponDetails.hasInfiniteAmmo == false)
            return;

        // if ammo in clip equals clip capacity then return
        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity)
            return;

        // Call the reload weapon event
        if (Input.GetKeyDown(KeyCode.R))
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
    }

    private void SwitchWeaponInput()
    {
        // Mouse Input
        if (Input.mouseScrollDelta.y > 0f)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex > player.weaponList.Count)
                currentWeaponIndex = 1;
            SetWeaponByIndex(currentWeaponIndex);
        }
        else if (Input.mouseScrollDelta.y < 0f)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 1)
                currentWeaponIndex = player.weaponList.Count;
            SetWeaponByIndex(currentWeaponIndex);
        }

        // Keyboard Input
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetWeaponByIndex(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetWeaponByIndex(2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetWeaponByIndex(3);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetWeaponByIndex(4);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SetWeaponByIndex(5);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SetWeaponByIndex(6);

        if (Input.GetKeyDown(KeyCode.Alpha7))
            SetWeaponByIndex(7);

        if (Input.GetKeyDown(KeyCode.Alpha8))
            SetWeaponByIndex(8);

        if (Input.GetKeyDown(KeyCode.Alpha9))
            SetWeaponByIndex(9);

        if (Input.GetKeyDown(KeyCode.Alpha0))
            SetWeaponByIndex(10);
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        if (weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    //===========================================================================
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