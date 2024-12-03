using UnityEngine;
using Unity.Cinemachine;
using De2Utils;

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
        targetCursor.position = De2Helper.GetMouseToWorldPosition();
    }

    //===========================================================================
    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cmgt_Player = new CinemachineTargetGroup.Target
        {
            Weight = 1.0f,
            Radius = 2.5f,
            Object = Player.Instance.transform
        };
        cinemachineTargetGroup.Targets.Add(cmgt_Player);
    }
}