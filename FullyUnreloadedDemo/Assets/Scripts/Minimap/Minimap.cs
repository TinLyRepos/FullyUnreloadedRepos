using NUnit.Framework;
using UnityEngine;

[DisallowMultipleComponent]
public class Minimap : SingletonMonobehaviour<Minimap>
{
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private GameObject playerIcon;

    //===========================================================================
    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(minimapCamera);
        Assert.IsNotNull(playerIcon);
    }

    private void Update()
    {
        Vector3 playerPosition = Player.Instance.Position;
        playerIcon.transform.position = playerPosition;
        minimapCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y, -10.0f);
    }

    //===========================================================================
    public void Initialize()
    {
        playerIcon.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetPlayerMinimapIcon();
    }
}