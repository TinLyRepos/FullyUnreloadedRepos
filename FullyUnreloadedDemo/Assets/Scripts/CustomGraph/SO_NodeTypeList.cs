using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeTypeList", menuName = "Scriptable Objects/Map Editor/Node Type List")]
public class SO_NodeTypeList : ScriptableObject
{
    [Header("NODE TYPE LIST")]
    public List<SO_NodeType> list = default;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtils.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
}