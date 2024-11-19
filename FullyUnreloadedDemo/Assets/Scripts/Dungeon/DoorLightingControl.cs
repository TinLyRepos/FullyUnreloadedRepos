using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    //===========================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeIn(door);
    }

    //===========================================================================
    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    //===========================================================================
    public void FadeIn(Door door)
    {
        // Create new material to fade in
        Material material = new Material(GameResources.Instance.shader_variableLit);

        if (!isLit)
        {
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
                StartCoroutine(FadeInRoutine(spriteRenderer, material));

            isLit = true;
        }
    }

    private IEnumerator FadeInRoutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;
        for (float i = 0.05f; i <= 1f; i += Settings.FADE_IN_TIME / Time.deltaTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }
        spriteRenderer.material = GameResources.Instance.material_lit;
    }
}