using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioClipComposite", menuName = "ScriptableObjects/AudioClipComposite")]
public class SO_AudioClipComposite : SO_AudioClip
{
    [SerializeField] private int soundCount = default;
    public int SoundCount => soundCount;

    private float singleSoundDuration = default;
    public float SingleSoundDuration { get => singleSoundDuration; set => singleSoundDuration = value; }
}