using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup = default;
    private Transform targetCursor = default;

    //===========================================================================
    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
        targetCursor = transform.GetChild(0);
    }

    private void FixedUpdate()
    {
        targetCursor.position = HelperUtilities.GetMouseToWorldPosition();
    }

    //===========================================================================
    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cmgt_Player = new CinemachineTargetGroup.Target
        {
            Weight = 1.0f, Radius = 2.5f,
            Object = GameManager.Instance.Player.transform
        };
        cinemachineTargetGroup.Targets.Add(cmgt_Player);
    }
}