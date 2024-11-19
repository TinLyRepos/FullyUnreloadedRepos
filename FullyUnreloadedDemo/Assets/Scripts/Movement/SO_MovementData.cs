using UnityEngine;

[CreateAssetMenu(fileName = "MovementData_", menuName = "Scriptable Objects/Movement/Movement Data")]
public class SO_MovementData : ScriptableObject
{
    [Header("MOVEMENT DATA")]
    public float minMoveSpeed = 8.0f;
    public float maxMoveSpeed = 8.0f;
    [Space]
    public float rollSpeed = default;    // for player
    public float rollDistance = default; // for player
    public float rollCDTime = default;   // for player

    //===========================================================================
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
            return minMoveSpeed;

        return Random.Range(minMoveSpeed, maxMoveSpeed);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this,
            nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCDTime), rollCDTime, false);
    }
#endif
}