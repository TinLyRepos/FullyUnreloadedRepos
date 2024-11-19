using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    [SerializeField] private GameObject weaponRotationPoint;

    private AimWeaponEvent aimWeaponEvent;

    //===========================================================================
    private void Awake()
    {
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable()
    {
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    //===========================================================================
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        Aim(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.aimAngle);
    }

    //===========================================================================
    private void Aim(AimDirection aimDirection, float aimAngle)
    {
        // Set angle of the weapon transform
        weaponRotationPoint.transform.eulerAngles = new Vector3(0.0f, 0.0f, aimAngle);

        switch (aimDirection)
        {
            case AimDirection.UpLeft:
            case AimDirection.Left:
                weaponRotationPoint.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
                break;
            case AimDirection.Up:
            case AimDirection.UpRight:
            case AimDirection.Down:
            case AimDirection.Right:
                weaponRotationPoint.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;
            default:
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRotationPoint), weaponRotationPoint);
    }
#endif
}