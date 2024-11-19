using UnityEngine;

[CreateAssetMenu(fileName = "NodeType", menuName = "Scriptable Objects/Map Editor/Node Type")]
public class SO_NodeType : ScriptableObject
{
    public string nodeTypeName = string.Empty;
    public bool showInMapEditor = true;

    public bool isBossRoom = default;
    public bool isCorridor = default;
    public bool isCorridor_Horizontal = default;
    public bool isCorridor_Vertical = default;
    public bool isEntrance = default;
    public bool isNone = default;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(nodeTypeName), nodeTypeName);
    }
#endif
}