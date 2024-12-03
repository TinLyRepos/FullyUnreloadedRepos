using NUnit.Framework;
using UnityEngine;

[DisallowMultipleComponent]
public class Player : SingletonMonobehaviour<Player>
{
    public PlayerAnimator Animator { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerWeapon Weapon { get; private set; }

    public Vector3 Position { get => transform.position; set => transform.position = value; }

    public bool MovementDisabled = false;

    public bool IsMoving { get; set; }
    public bool IsRolling { get; set; }

    //===========================================================================
    protected override void Awake()
    {
        base.Awake();

        Animator = GetComponent<PlayerAnimator>();
        Controller = GetComponent<PlayerController>();
        Weapon = GetComponent<PlayerWeapon>();

        Assert.IsNotNull(Animator);
        Assert.IsNotNull(Controller);
        Assert.IsNotNull(Weapon);
    }

    //===========================================================================
    public void Initialize(SO_PlayerData playerData)
    {

    }

    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
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
        // weaponList.Add(weapon);

        // Set weapon position in list
        // weapon.weaponListPosition = weaponList.Count;

        // Set the added weapon as active
        // setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }
}