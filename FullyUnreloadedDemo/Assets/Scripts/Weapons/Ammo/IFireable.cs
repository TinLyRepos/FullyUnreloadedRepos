using UnityEngine;

public interface IFireable
{
    void InitialiseAmmo(SO_AmmoData ammoDetails, 
        float aimAngle, float weaponAimAngle, float ammoSpeed, 
        Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false);

    GameObject GetGameObject();
}