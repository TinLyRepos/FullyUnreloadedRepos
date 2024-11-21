using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<GameResources>("GameResources");

            return instance;
        }
    }

    [Header("NODE TYPE LIST")]
    public SO_NodeTypeList nodeTypeList = default;

    [Header("RUNTIME MATERIALS & SHADERS")]
    public Material material_dimmed = default;
    public Material material_lit = default;
    public Shader shader_VariableLit = default;
    public Shader shader_Materialize;

    [Header("PLAYER")]
    public SO_CurrentPlayerData currentPlayerData = default;

    [Header("MUSIC")]
    public AudioMixerGroup musicMasterMixerGroup;
    // public MusicTrackSO mainMenuMusic;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;

    [Header("SOUNDS")]
    public AudioMixerGroup soundsMasterMixerGroup;
    public SoundEffectSO doorOpenCloseSoundEffect;
    public SoundEffectSO tableFlip;
    public SoundEffectSO chestOpen;
    public SoundEffectSO healthPickup;
    public SoundEffectSO weaponPickup;
    public SoundEffectSO ammoPickup;

    [Header("ASTAR TILES")]
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase preferredEnemyPathTile;

    [Header("UI")]
    public GameObject heartPrefab;
    public GameObject ammoIconPrefab;
    public GameObject scorePrefab;

    [Header("CHESTS")]
    public GameObject chestItemPrefab;
    public Sprite heartIcon;
    public Sprite bulletIcon;

    [Header("MINIMAP")]
    public GameObject minimapSkullPrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(nodeTypeList), nodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayerData), currentPlayerData);
        HelperUtilities.ValidateCheckNullValue(this, nameof(material_dimmed), material_dimmed);
        HelperUtilities.ValidateCheckNullValue(this, nameof(material_lit), material_lit);
        HelperUtilities.ValidateCheckNullValue(this, nameof(shader_VariableLit), shader_VariableLit);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);

        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
    }
#endif
}