using System.Collections.Generic;
using UnityEngine;
using De2Utils;

public class PlayerAnimator : MonoBehaviour
{
    [Header("SPRITE RENDERER CACHE")]
    [SerializeField] private SpriteRenderer playerSprite = default;

    [Header("ANIMATION DATA")]
    [SerializeField] private SO_AnimationList animationList_TheCowgirl = default;

    private SO_AnimationList currentAnimationList = default;
    private List<Sprite> animationSpriteList = default;
    private int animationFrame = default;
    private float animationSpeed = default;
    private float animationTimer = default;

    //===========================================================================
    private void OnEnable()
    {
        currentAnimationList = animationList_TheCowgirl;
    }

    private void Update()
    {
        FlipSprite();

        AssignSpriteAnimation();

        PlayAnimation(playerSprite, animationSpriteList);
    }

    //===========================================================================
    private void FlipSprite()
    {
        if (Player.Instance.Position.x > De2Helper.GetMouseToWorldPosition().x)
        {
            playerSprite.flipX = true;
        }
        else
        {
            playerSprite.flipX = false;
        }
    }

    private void SetPlayerAnimationSpeed()
    {
        // Set animator speed to match movement speed
        // player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void AssignSpriteAnimation()
    {
        SO_Animation animationClip = null;

        if (Player.Instance.Controller.MovementVector == Vector2.zero)
        {
            switch (Player.Instance.Controller.AimDirection)
            {
                case AimDirection.Up:
                    animationClip = currentAnimationList.GetAnimation(AnimationType.IdleUp);
                    break;
                case AimDirection.UpRight:
                case AimDirection.UpLeft:
                    animationClip = currentAnimationList.GetAnimation(AnimationType.IdleUpSide);
                    break;
                case AimDirection.Down:
                    animationClip = currentAnimationList.GetAnimation(AnimationType.IdleDown);
                    break;
                case AimDirection.Left:
                case AimDirection.Right:
                    animationClip = currentAnimationList.GetAnimation(AnimationType.IdleDownSide);
                    break;
            }
        }
        else
        {
            if (Player.Instance.IsRolling)
            {
                switch (Player.Instance.Controller.AimDirection)
                {
                    case AimDirection.Up:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.RollUp);
                        break;
                    case AimDirection.UpRight:
                    case AimDirection.UpLeft:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.RollUpSide);
                        break;
                    case AimDirection.Down:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.RollDown);
                        break;
                    case AimDirection.Left:
                    case AimDirection.Right:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.RollDownSide);
                        break;
                }
            }
            else
            {
                switch (Player.Instance.Controller.AimDirection)
                {
                    case AimDirection.Up:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.MoveUp);
                        break;
                    case AimDirection.UpRight:
                    case AimDirection.UpLeft:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.MoveUpSide);
                        break;
                    case AimDirection.Down:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.MoveDown);
                        break;
                    case AimDirection.Left:
                    case AimDirection.Right:
                        animationClip = currentAnimationList.GetAnimation(AnimationType.MoveDownSide);
                        break;
                }
            }
        }

        // Swap only if animations have changed
        if (animationClip != null && animationClip.Sprites != animationSpriteList)
            UpdateAnimationClip(ref animationSpriteList, animationClip);
    }

    private void UpdateAnimationClip(ref List<Sprite> currentSprites, SO_Animation animationData)
    {
        currentSprites = animationData.Sprites;
        animationSpeed = animationData.Speed;
        animationTimer = 0.0f;
        animationFrame = 0;
    }

    private void PlayAnimation(SpriteRenderer renderer, List<Sprite> sprites)
    {
        if (animationFrame == 0)
            renderer.sprite = sprites[animationFrame];

        animationTimer += Time.deltaTime;
        while (animationTimer >= animationSpeed)
        {
            animationTimer -= animationSpeed;
            animationFrame = (animationFrame + 1) % sprites.Count;
            renderer.sprite = sprites[animationFrame];
        }
    }
}