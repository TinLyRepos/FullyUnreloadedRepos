using UnityEngine;

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
    [Space]
    [Header("RUNTIME MATERIALS & SHADERS")]
    public Material material_dimmed = default;
    public Material material_lit = default;
    public Shader shader_variableLit = default;
    [Space]
    [Header("PLAYER")]
    public SO_CurrentPlayerData currentPlayerData = default;
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(nodeTypeList), nodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayerData), currentPlayerData);
        HelperUtilities.ValidateCheckNullValue(this, nameof(material_dimmed), material_dimmed);
        HelperUtilities.ValidateCheckNullValue(this, nameof(material_lit), material_lit);
        HelperUtilities.ValidateCheckNullValue(this, nameof(shader_variableLit), shader_variableLit);
    }
#endif
}