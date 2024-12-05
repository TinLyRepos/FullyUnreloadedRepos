using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/Player/Player Data")]
public class SO_PlayerData : ScriptableObject
{
    [Header("BASE DATA")]
    [SerializeField] private string characterName;

    [Header("ANIMATION")]
    [SerializeField] private SO_AnimationList animationList;
    [SerializeField] private Sprite miniMapIcon;
    [SerializeField] private Sprite handSprite;

    [Header("GAMEPLAY")]
    [SerializeField] private int baseMaxHealth;
    [SerializeField] private float baseMoveSpeed;

    [Header("WEAPON")]
    [SerializeField] private SO_WeaponData weaponStarter;

    public string Name => characterName;
    public SO_AnimationList AnimationList => animationList;
    public Sprite MiniMapIcon => miniMapIcon;
    public Sprite HandSprite => handSprite;
    public int BaseMaxHealth => baseMaxHealth;
    public float BaseMoveSpeed => baseMoveSpeed;
    public SO_WeaponData WeaponStarter => weaponStarter;

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {

    }
#endif
}