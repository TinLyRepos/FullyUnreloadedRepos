using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/Player/Player Data")]
public class SO_PlayerData : ScriptableObject
{
    [Header("BASE DATA")]
    [SerializeField] private string characterName = string.Empty;
    [SerializeField] private GameObject characterPrefab = default;

    [Header("ANIMATION")]
    [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;
    [SerializeField] private Sprite playerMiniMapSprite;
    [SerializeField] private Sprite playerHandSprite;

    [Header("GAMEPLAY")]
    [SerializeField] private int playerBaseMaxHealth;
    public bool isImmuneAfterHit = false;
    public float hitImmunityTime;

    [Header("WEAPON")]
    [SerializeField] private WeaponDetailsSO startingWeapon;
    [SerializeField] private List<WeaponDetailsSO> startingWeaponList;

    public string Name => characterName;
    public GameObject Prefab => characterPrefab;
    public RuntimeAnimatorController RuntimeAnimatorController => runtimeAnimatorController;
    public Sprite MiniMapIcon => playerMiniMapSprite;
    public Sprite HandSprite => playerHandSprite;
    public int BaseMaxHealth => playerBaseMaxHealth;
    public WeaponDetailsSO StartingWeapon => startingWeapon;
    public List<WeaponDetailsSO> StartingWeaponList => startingWeaponList;

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(characterName), characterName);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerBaseMaxHealth), playerBaseMaxHealth, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMiniMapSprite), playerMiniMapSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);
    }
#endif
}