using UnityEngine;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
    // Attach this class to environment game objects whose lighting gets faded in
    [Header("REFERENCE")]
    public SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
    }
#endif
}