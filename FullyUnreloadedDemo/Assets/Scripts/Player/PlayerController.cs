using UnityEngine;
using De2Utils;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB2D = default;

    // MOVEMENT
    private float moveSpeed = 5.0f;

    // ROLL
    private float rollDistance = 4.48f;  // Total roll distance
    private float rollSpeed = 7.0f;     // Speed during roll
    private Vector2 rollDirection = Vector2.zero;
    private float rollRemainingDistance = 0f;

    private float rollCooldown = 0.5f;
    private float currentRollCooldown = 0f;

    //===========================================================================
    public Vector2 MovementVector { get; private set; }

    public AimDirection AimDirection { get; private set; }

    //===========================================================================
    private void Awake()
    {
        playerRB2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        CustomPlayerInputEvent.Instance.OnInput_Move += OnInput_Move_Handler;
        CustomPlayerInputEvent.Instance.OnInput_Roll += OnInput_Roll_Handler;
    }

    private void Update()
    {
        UpdateRollCooldown();

        if (Player.Instance.IsRolling)
        {
            PerformRoll();
        }
        else
        {
            AimPlayer();

            PerformMove();
        }
    }

    private void OnDisable()
    {
        if (CustomPlayerInputEvent.Instance == null)
            return;

        CustomPlayerInputEvent.Instance.OnInput_Move -= OnInput_Move_Handler;
        CustomPlayerInputEvent.Instance.OnInput_Roll -= OnInput_Roll_Handler;
    }

    //===========================================================================
    private void OnInput_Move_Handler(object sender, Vector2 e)
    {
        if (Player.Instance.IsRolling)
            return;

        MovementVector = e.normalized;
    }

    private void OnInput_Roll_Handler(object sender, System.EventArgs e)
    {
        if (MovementVector == Vector2.zero)
            return;

        if (Player.Instance.IsRolling || currentRollCooldown > 0.0f)
            return;

        TriggerRoll();
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

    private void UpdateRollCooldown()
    {
        if (currentRollCooldown > 0f)
            currentRollCooldown -= Time.deltaTime;
    }

    private void TriggerRoll()
    {
        Player.Instance.IsRolling = true;
        Player.Instance.MovementDisabled = true;
        Player.Instance.Weapon.SetWeaponActive(false);

        rollDirection = MovementVector;
        rollRemainingDistance = rollDistance;
        currentRollCooldown = rollCooldown;

        playerRB2D.linearVelocity = rollDirection * rollSpeed;
    }

    private void PerformRoll()
    {
        float distanceToTravel = rollSpeed * Time.deltaTime;
        if (rollRemainingDistance > distanceToTravel)
        {
            rollRemainingDistance -= distanceToTravel;
            playerRB2D.linearVelocity = rollDirection * rollSpeed;
        }
        else
        {
            StopRoll();
        }
    }

    private void StopRoll()
    {
        Player.Instance.IsRolling = false;
        Player.Instance.MovementDisabled = false;
        Player.Instance.Weapon.SetWeaponActive(true);

        rollRemainingDistance = 0f;
        playerRB2D.linearVelocity = Vector2.zero;
    }

    private void PerformMove()
    {
        if (Player.Instance.MovementDisabled)
            return;

        playerRB2D.linearVelocity = MovementVector * moveSpeed;
    }
}