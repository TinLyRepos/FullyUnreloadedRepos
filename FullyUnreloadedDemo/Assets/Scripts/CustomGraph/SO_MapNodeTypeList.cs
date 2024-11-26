using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeTypeList", menuName = "Scriptable Objects/Map Editor/Node Type List")]
public class SO_MapNodeTypeList : ScriptableObject
{
    [Header("NODE TYPE LIST")]
    public List<SO_MapNodeType> list = default;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
}