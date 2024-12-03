using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimation", menuName = "ScriptableObjects/Animation")]
public class SO_Animation : ScriptableObject
{
    [SerializeField] private float animationSpeed = 0.5f;
    public float Speed => animationSpeed;

    [SerializeField] private List<Sprite> animationSprites = default;
    public List<Sprite> Sprites => animationSprites;
}