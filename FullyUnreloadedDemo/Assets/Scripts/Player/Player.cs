using NUnit.Framework;
using UnityEngine;

[DisallowMultipleComponent]
public class Player : SingletonMonobehaviour<Player>
{
    public PlayerAnimator Animator { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerWeapon Weapon { get; private set; }

    public Vector3 Position { get => transform.position; set => transform.position = value; }

    public bool MovementDisabled { get; set; }
    public bool IsMoving { get; set; }
    public bool IsRolling { get; set; }

    public SO_PlayerData PlayerData { get; set; }

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
        Animator.SetAnimationList(playerData.AnimationList);
        Controller.SetBaseMoveSpeed(playerData.BaseMoveSpeed);

        Weapon.SetWeaponStarter(playerData.WeaponStarter);
    }
}