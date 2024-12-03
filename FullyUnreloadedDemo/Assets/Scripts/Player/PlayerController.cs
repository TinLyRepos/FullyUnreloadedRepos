using UnityEngine;
using De2Utils;
using UnityEngine.UIElements;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    // BASIC MOVEMENT
    private Rigidbody2D playerRB2D = default;
    private float moveSpeed = default;

    [HideInInspector] public bool isPlayerRolling = false;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private float playerRollCDTimer = 0.0f;
    private bool isPlayerMovementDisabled = false;

    // Weapon
    [Header("WEAPON CONFIG")]
    [SerializeField] private Transform playerWeapon = default;
    [SerializeField] private SpriteRenderer weaponSprite = default;

    private Vector2 weaponPos = new Vector2(0.375f, 0.3125f);
    private Vector2 weaponPosFlip = new Vector2(-0.375f, 0.3125f);

    private int currentWeaponIndex = 1;
    private bool leftMouseDownPreviousFrame = false;

    //===========================================================================
    public Vector2 MoveVector { get; private set; }
    public AimDirection AimDirection { get; private set; }

    //===========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Stop player roll Coroutine
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Stop player roll Coroutine
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }

    //===========================================================================
    private void Awake()
    {
        playerRB2D = GetComponent<Rigidbody2D>();
        moveSpeed = 5.0f;
    }

    private void OnEnable()
    {
        CustomPlayerInputEvent.Instance.OnInput_Move += OnInput_Move_Handler;
    }

    private void Update()
    {
        AimPlayer();

        AimWeaponInput();

        MovePlayer();
    }

    private void OnDisable()
    {
        if (CustomPlayerInputEvent.Instance == null)
            return;
        CustomPlayerInputEvent.Instance.OnInput_Move -= OnInput_Move_Handler;
    }

    //===========================================================================
    private void OnInput_Move_Handler(object sender, Vector2 e)
    {
        MoveVector = e.normalized;
    }

    //===========================================================================
    private void AimPlayer()
    {
        // Get player aiming vector
        Vector3 playerDirection = De2Helper.GetMouseToWorldPosition() - Player.Instance.Position;

        // Convert player aiming vector to aim angle
        float aimAngle = HelperUtilities.GetAngleFromVector(playerDirection);

        // Convert player aiming angle to aim direction
        AimDirection = De2Helper.GetAimDirection(aimAngle);
    }

    private void AimWeaponInput()
    {
        // Get weapon aiming vector
        Vector3 weaponDirection = De2Helper.GetMouseToWorldPosition() - playerWeapon.position;

        // Convert weapon aiming vector to aim angle
        float aimAngle = HelperUtilities.GetAngleFromVector(weaponDirection);

        // Set angle of the weapon transform
        playerWeapon.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, aimAngle));

        // Set Player Weapon based on AimPlayer()
        switch (AimDirection)
        {
            case AimDirection.UpLeft:
            case AimDirection.Left:
                playerWeapon.localPosition = weaponPosFlip;
                weaponSprite.flipY = true;
                break;
            case AimDirection.Up:
            case AimDirection.UpRight:
            case AimDirection.Right:
            case AimDirection.Down:
                playerWeapon.localPosition = weaponPos;
                weaponSprite.flipY = false;
                break;
            default:
                break;
        }
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        // Fire when left mouse button is clicked
        if (Input.GetMouseButton(0))
        {
            // Trigger fire weapon event
            // player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void ReloadWeaponInput()
    {
        //Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        //// if current weapon is reloading return
        //if (currentWeapon.isWeaponReloading)
        //    return;

        //// remaining ammo is less than clip capacity then return and not infinite ammo then return
        //if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && 
        //    currentWeapon.weaponDetails.hasInfiniteAmmo == false)
        //    return;

        //// if ammo in clip equals clip capacity then return
        //if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity)
        //    return;

        //// Call the reload weapon event
        //if (Input.GetKeyDown(KeyCode.R))
        //    player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
    }

    private void SwitchWeaponInput()
    {
        //// Mouse Input
        //if (Input.mouseScrollDelta.y > 0f)
        //{
        //    currentWeaponIndex++;
        //    if (currentWeaponIndex > player.weaponList.Count)
        //        currentWeaponIndex = 1;
        //    SetWeaponByIndex(currentWeaponIndex);
        //}
        //else if (Input.mouseScrollDelta.y < 0f)
        //{
        //    currentWeaponIndex--;
        //    if (currentWeaponIndex < 1)
        //        currentWeaponIndex = player.weaponList.Count;
        //    SetWeaponByIndex(currentWeaponIndex);
        //}

        //// Keyboard Input
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    SetWeaponByIndex(1);

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //    SetWeaponByIndex(2);

        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //    SetWeaponByIndex(3);

        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //    SetWeaponByIndex(4);

        //if (Input.GetKeyDown(KeyCode.Alpha5))
        //    SetWeaponByIndex(5);

        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //    SetWeaponByIndex(6);

        //if (Input.GetKeyDown(KeyCode.Alpha7))
        //    SetWeaponByIndex(7);

        //if (Input.GetKeyDown(KeyCode.Alpha8))
        //    SetWeaponByIndex(8);

        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //    SetWeaponByIndex(9);

        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //    SetWeaponByIndex(10);
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        //if (weaponIndex - 1 < player.weaponList.Count)
        //{
        //    currentWeaponIndex = weaponIndex;
        //    player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        //}
    }

    //===========================================================================
    private void MovePlayer()
    {
        playerRB2D.linearVelocity = MoveVector * moveSpeed;
    }

    private void UseItemInput()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    float useItemRadius = 2f;

        //    // Get any 'Useable' item near the player
        //    Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

        //    // Loop through detected items to see if any are 'useable'
        //    foreach (Collider2D collider2D in collider2DArray)
        //    {
        //        IUseable iUseable = collider2D.GetComponent<IUseable>();

        //        if (iUseable != null)
        //        {
        //            iUseable.UseItem();
        //        }
        //    }
        //}
    }

    //===========================================================================
    private void PlayerRollCooldownTimer()
    {
        if (playerRollCDTimer >= 0f)
            playerRollCDTimer -= Time.deltaTime;
    }
}