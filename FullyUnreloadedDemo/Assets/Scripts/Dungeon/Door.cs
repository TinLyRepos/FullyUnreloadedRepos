using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private BoxCollider2D doorCollider;
    [HideInInspector] public bool isBossRoomDoor = false;

    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    //===========================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
            OpenDoor();
    }

    //===========================================================================
    private void Awake()
    {
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();

        doorCollider.enabled = false;
    }

    private void OnEnable()
    {
        // When the parent gameobject is disabled (when the player moves far enough away from the
        // room) the animator state gets reset. Therefore we need to restore the animator state.
        animator.SetBool(Settings.open, isOpen);
    }

    //===========================================================================
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            // Set open parameter in animator
            animator.SetBool(Settings.open, true);

            // play sound effect
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
        }
    }

    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        // set open to false to close door
        animator.SetBool(Settings.open, false);
    }

    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion
}