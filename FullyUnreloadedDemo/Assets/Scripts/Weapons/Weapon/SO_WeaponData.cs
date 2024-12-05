using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Scriptable Objects/Weapon/Weapon Data")]
public class SO_WeaponData : ScriptableObject
{
    [Header("WEAPON DATA")]
    [SerializeField] private string weaponName = string.Empty;
    [SerializeField] private Sprite weaponSprite = default;

    [Header("WEAPON GAME OBJECT")]
    [SerializeField] private SO_AmmoData ammoData = default;
    [SerializeField] private Vector2 weaponPosition = Vector2.zero;
    [SerializeField] private Vector2 shootPosition = Vector2.zero;

    [Header("WEAPON SOUND")]
    [SerializeField] private SoundEffectSO soundEffectFire;
    [SerializeField] private SoundEffectSO soundEffectReload;

    [Header("WEAPON BASE STATS")]
    [SerializeField] private bool hasInfiniteAmmo = false;
    [SerializeField] private int clipCapacity = 6;
    [SerializeField] private int ammoCapacity = 100;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float reloadTime = 0f;

    public string Name => weaponName;
    public Sprite Sprite => weaponSprite;

    public SO_AmmoData AmmoData => ammoData;
    public Vector2 WeaponPosition => weaponPosition;
    public Vector2 ShootPosition => shootPosition;

    public SoundEffectSO SoundEffectFire => soundEffectFire;
    public SoundEffectSO SoundEffectReload => soundEffectReload;

    public bool HasInfiniteAmmo => hasInfiniteAmmo;
    public int AmmoCapacity => ammoCapacity;
    public int ClipCapacity => clipCapacity;
    public float FireRate => fireRate;
    public float ReloadTime => reloadTime;

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {

    }
#endif
}