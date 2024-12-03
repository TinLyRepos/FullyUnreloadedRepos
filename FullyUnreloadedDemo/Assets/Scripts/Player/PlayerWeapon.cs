using UnityEngine;
using De2Utils;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Transform playerWeapon = default;
    [SerializeField] private SpriteRenderer weaponSprite = default;

    private int currentWeaponIndex = 1;
    private bool leftMouseDownPreviousFrame = false;

    //===========================================================================
    public Vector2 WeaponPositionL = new Vector2(0.5f, 0.375f);

    public Vector2 WeaponPositionR = new Vector2(-0.5f, 0.375f);

    //===========================================================================
    private void Update()
    {
        AimWeapon();
    }

    //===========================================================================
    private void AimWeapon()
    {
        // Get weapon aiming vector
        Vector3 weaponDirection = De2Helper.GetMouseToWorldPosition() - playerWeapon.position;

        // Convert weapon aiming vector to aim angle
        float aimAngle = HelperUtilities.GetAngleFromVector(weaponDirection);

        // Set angle of the weapon transform
        playerWeapon.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, aimAngle));

        // Set Player Weapon based on AimPlayer()
        switch (Player.Instance.Controller.AimDirection)
        {
            case AimDirection.UpLeft:
            case AimDirection.Left:
                playerWeapon.localPosition = WeaponPositionR;
                weaponSprite.flipY = true;
                break;
            case AimDirection.UpRight:
            case AimDirection.Right:
                playerWeapon.localPosition = WeaponPositionL;
                weaponSprite.flipY = false;
                break;
            case AimDirection.Up:
            case AimDirection.Down:
                if (Player.Instance.Position.x > De2Helper.GetMouseToWorldPosition().x)
                {
                    playerWeapon.localPosition = WeaponPositionR;
                    weaponSprite.flipY = true;
                }
                else
                {
                    playerWeapon.localPosition = WeaponPositionL;
                    weaponSprite.flipY = false;
                }
                break;
            default:
                break;
        }

        // Set Player Weapon Sprite render order based on AimPlayer()
        switch (Player.Instance.Controller.AimDirection)
        {
            case AimDirection.UpLeft:
            case AimDirection.Up:
            case AimDirection.UpRight:
                weaponSprite.sortingOrder = 0;
                break;
            case AimDirection.Left:
            case AimDirection.Down:
            case AimDirection.Right:
                weaponSprite.sortingOrder = 10;
                break;
            default:
                break;
        }
    }

    //===========================================================================
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
    public void SetWeaponActive(bool active)
    {
        weaponSprite.enabled = active;
    }
}