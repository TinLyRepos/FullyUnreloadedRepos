using UnityEngine;

public class SimpleAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = default;
    [Space]
    [SerializeField] private Sprite defaultSprite = default;
    [SerializeField] private Sprite[] sprites = default;
    [SerializeField] private float animationSpeed = default;

    private bool playAnimation = true;
    private int currentSpriteIndex = 0;
    private float animationTimer = 0f;

    //===========================================================================
    private void Update()
    {
        if (playAnimation == false)
            return;

        PlayAnimation();
    }

    //===========================================================================
    private void PlayAnimation()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= animationSpeed)
        {
            animationTimer -= animationSpeed;
            currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[currentSpriteIndex];
        }
    }

    //===========================================================================
    public void SetPlayAnimation(bool active)
    {
        if (playAnimation == false && active == true)
        {
            currentSpriteIndex = 0;
        }
        if (active == false)
        {
            spriteRenderer.sprite = defaultSprite;
        }

        playAnimation = active;
    }
}