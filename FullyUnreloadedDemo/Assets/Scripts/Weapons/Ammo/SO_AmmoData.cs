using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class SO_AmmoData : ScriptableObject
{
    [Space]
    [Header("BASIC AMMO DETAILS")]
    public string ammoName;
    public bool isPlayerAmmo;
    [Space]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]
    public Sprite ammoSprite;
    public GameObject[] ammoPrefabArray;
    public Material ammoMaterial;
    public float ammoChargeTime = 0.1f;
    public Material ammoChargeMaterial;
    [Space]
    [Header("AMMO HIT EFFECT")]
    public AmmoHitEffectSO ammoHitEffect;
    [Space]
    [Header("AMMO BASE PARAMETERS")]
    public int ammoDamage = 1;
    public float ammoSpeedMin = 20f;
    public float ammoSpeedMax = 20f;
    public float ammoRange = 20f;
    public float ammoRotationSpeed = 1f;
    [Space]
    [Header("AMMO SPREAD DETAILS")]
    public float ammoSpreadMin = 0f;
    public float ammoSpreadMax = 0f;
    [Space]
    [Header("AMMO SPAWN DETAILS")]
    public int ammoSpawnAmountMin = 1;
    public int ammoSpawnAmountMax = 1;
    public float ammoSpawnIntervalMin = 0f;
    public float ammoSpawnIntervalMax = 0f;
    [Space]
    [Header("AMMO TRAIL DETAILS")]
    public bool isAmmoTrail = false;
    public float ammoTrailTime = 3f;
    public Material ammoTrailMaterial;
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    [Range(0f, 1f)] public float ammoTrailEndWidth;

#if UNITY_EDITOR
    private void OnValidate()
    {

    }
#endif
}