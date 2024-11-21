using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    [Header("WEAPON DATA")]
    public string weaponName;
    public Sprite weaponSprite;
    [Space]
    [Header("WEAPON GAMEOBJECT CONFIG")]
    public Vector3 weaponShootPosition;
    public AmmoDetailsSO weaponCurrentAmmo;
    public WeaponShootEffectSO weaponShootEffect;
    [Space]
    [Header("WEAPON SOUND CONFIG")]
    public SoundEffectSO soundEffectFire;
    public SoundEffectSO soundEffectReload;
    [Space]
    [Header("WEAPON STATS CONFIG")]
    public bool hasInfiniteAmmo = false;
    public bool hasInfiniteClipCapacity = false;
    public int weaponClipAmmoCapacity = 6;
    public int weaponAmmoCapacity = 100;
    public float weaponFireRate = 0.2f;
    public float weaponPrechargeTime = 0f;
    public float weaponReloadTime = 0f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }
#endif
}