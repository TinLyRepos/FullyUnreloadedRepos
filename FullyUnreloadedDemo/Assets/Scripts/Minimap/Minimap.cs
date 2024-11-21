using UnityEngine;
using Unity.Cinemachine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private Camera minimapCamera;
    private Transform playerTransform;

    //===========================================================================
    private void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;

        // Set minimap player icon
        SpriteRenderer spriteRenderer = playerIcon.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMinimapIcon();
    }

    private void Update()
    {
        // Move the minimap player to follow the player
        if (playerTransform != null && playerIcon != null)
        {
            Vector3 playerPos = playerTransform.position;
            playerIcon.transform.position = playerPos;
            minimapCamera.transform.position = new Vector3(playerPos.x, playerPos.y, -10.0f);
        }
    }

    //===========================================================================
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerIcon), playerIcon);
    }
#endif
}