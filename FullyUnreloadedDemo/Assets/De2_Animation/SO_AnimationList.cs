using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Animation List")]
public class SO_AnimationList : ScriptableObject
{
    [Header("IDLE")]
    [SerializeField] private SO_Animation idle_down = default;
    [SerializeField] private SO_Animation idle_down_side = default;
    [SerializeField] private SO_Animation idle_up = default;
    [SerializeField] private SO_Animation idle_up_side = default;
    [Header("MOVE")]
    [SerializeField] private SO_Animation move_down = default;
    [SerializeField] private SO_Animation move_down_side = default;
    [SerializeField] private SO_Animation move_up = default;
    [SerializeField] private SO_Animation move_up_side = default;
    [Header("ROLL")]
    [SerializeField] private SO_Animation roll_down = default;
    [SerializeField] private SO_Animation roll_down_side = default;
    [SerializeField] private SO_Animation roll_up = default;
    [SerializeField] private SO_Animation roll_up_side = default;

    //===========================================================================
    public SO_Animation GetAnimation(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.IdleDown:        return idle_down;
            case AnimationType.IdleDownSide:    return idle_down_side;
            case AnimationType.IdleUp:          return idle_up;
            case AnimationType.IdleUpSide:      return idle_up_side;
            case AnimationType.MoveDown:        return move_down;
            case AnimationType.MoveDownSide:    return move_down_side;
            case AnimationType.MoveUp:          return move_up;
            case AnimationType.MoveUpSide:      return move_up_side;
            case AnimationType.RollDown:        return roll_down;
            case AnimationType.RollDownSide:    return roll_down_side;
            case AnimationType.RollUp:          return roll_up;
            case AnimationType.RollUpSide:      return roll_up_side;
            default:
                Assert.Fail("SO_AnimationList: Invalid Animation Type");
                return null;
        }
    }

    public void Copy(SO_AnimationList list)
    {

    }

    public void Clear()
    {

    }
}