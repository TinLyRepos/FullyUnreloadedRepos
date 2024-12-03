using UnityEngine;
using Unity.Cinemachine;

public class UpdateConfineBoundingShape : MonoBehaviour
{
    private void Start()
    {
        UpdateBoundingShape();
    }

    private void UpdateBoundingShape()
    {
        // Get Polygon Collider
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(StringTags.VCamBoundsConfiner).GetComponent<PolygonCollider2D>();
        
        CinemachineConfiner2D cinemachineConfiner2D = GetComponent<CinemachineConfiner2D>();

        cinemachineConfiner2D.BoundingShape2D = polygonCollider2D;

        // Clear cache
        cinemachineConfiner2D.InvalidateBoundingShapeCache();
    }
}