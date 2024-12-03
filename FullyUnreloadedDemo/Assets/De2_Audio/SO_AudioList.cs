using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioList", menuName = "ScriptableObjects/AudioList")]
public class SO_AudioList : ScriptableObject
{
    [SerializeField] SO_AudioClip[] audioList = default;
    public SO_AudioClip[] AudioList => audioList;
}