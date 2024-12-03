using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioClip", menuName = "ScriptableObjects/AudioClip")]
public class SO_AudioClip : ScriptableObject
{
    [SerializeField] private string clipName = string.Empty;
    public string AudioClipName => clipName;

    [SerializeField] private AudioClip audioClip;
    public AudioClip AudioClip => audioClip;
}