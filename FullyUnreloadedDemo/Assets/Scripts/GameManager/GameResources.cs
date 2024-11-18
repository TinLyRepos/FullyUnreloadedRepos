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
    [Header("MATERIALS")]
    public Material dimmedMaterial = default;
}