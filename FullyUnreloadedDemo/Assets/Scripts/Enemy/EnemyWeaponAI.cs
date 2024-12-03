using De2Utils;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    // Select the layers that the enemy bullets will hit
    [SerializeField] private LayerMask layerMask;
    //Populate this with the WeaponShootPosition child gameobject transform
    [SerializeField] private Transform weaponShootPosition;

    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetails = enemy.enemyDetails;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        // Update timers
        firingIntervalTimer -= Time.deltaTime;

        // Interval Timer
        if (firingIntervalTimer < 0f)
        {
            if (firingDurationTimer >= 0)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                // Reset timers
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }
    }

    /// Calculate a random weapon shoot duration between the min and max values
    private float WeaponShootDuration()
    {
        // Calculate a random weapon shoot duration
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }

    /// Calculate a random weapon shoot interval between the min and max values
    private float WeaponShootInterval()
    {
        // Calculate a random weapon shoot interval
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }

    /// Fire the weapon
    private void FireWeapon()
    {
        // Player distance
        Vector3 playerDirectionVector = Player.Instance.Position - transform.position;

        // Calculate direction vector of player from weapon shoot position
        Vector3 weaponDirection = (Player.Instance.Position - weaponShootPosition.position);

        // Get weapon to player angle
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // Get enemy to player angle
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        // Set enemy aim direction
        AimDirection enemyAimDirection = De2Helper.GetAimDirection(enemyAngleDegrees);

        // Trigger weapon aim event
        enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

        // Only fire if enemy has a weapon
        if (enemyDetails.enemyWeapon != null)
        {
            // Get ammo range
            float enemyAmmoRange = enemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;

            // Is the player in range
            if (playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                // Does this enemy require line of sight to the player before firing?
                if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) return;

                // Trigger fire weapon event
                enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);
            }
        }
    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirection, enemyAmmoRange, layerMask);
        
        if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
            return true;

        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
    }
#endif
}