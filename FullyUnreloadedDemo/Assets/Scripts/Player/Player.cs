using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(ActiveWeapon))]

[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
public class Player : MonoBehaviour
{
    [HideInInspector] public SO_PlayerData playerData;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    [HideInInspector] public Health health;
    [HideInInspector] public ActiveWeapon activeWeapon;

    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;

    public List<Weapon> weaponList = new List<Weapon>();

    //===========================================================================
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        health = GetComponent<Health>();
        activeWeapon = GetComponent<ActiveWeapon>();

        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    //===========================================================================
    public void Initialize(SO_PlayerData playerData)
    {
        this.playerData = playerData;

        SetPlayerHealth();

        CreatePlayerStartingWeapons();
    }

    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerData.BaseMaxHealth);
    }

    private void CreatePlayerStartingWeapons()
    {
        // Clear list
        weaponList.Clear();

        // Populate weapon list from starting weapons
        foreach (WeaponDetailsSO weaponDetails in playerData.StartingWeaponList)
        {
            Weapon weapon = new Weapon()
            {
                weaponDetails = weaponDetails,
                weaponReloadTimer = 0f,
                weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity,
                weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity,
                isWeaponReloading = false
            };

            // Add the weapon to the list
            weaponList.Add(weapon);

            // Set weapon position in list
            weapon.weaponListPosition = weaponList.Count;

            // Set the added weapon as active
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }
}