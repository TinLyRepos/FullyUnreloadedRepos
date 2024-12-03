using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bmgSource = default;
    [SerializeField] private AudioSource sfxSource = default;
    [Space]
    [SerializeField] private SO_AudioList audioList = default;
    [SerializeField] private SO_AudioList audioListComposite = default;
    [SerializeField] private Dictionary<string, List<AudioClip>> audioSubclipDictionary = default;

    //===========================================================================
    private void Awake()
    {
        if (bmgSource == null)
            Debug.LogWarning("AudioManager: Cannot find BMG_Source");

        if (sfxSource == null)
            Debug.LogWarning("AudioManager: Cannot find SFX_Source");

        // Create sub-clips for all audio clips in the audio list
        audioSubclipDictionary = new();
        foreach (SO_AudioClipComposite soundData in audioListComposite.AudioList)
        {
            List<AudioClip> subclips = CreateSubClips(soundData);
            audioSubclipDictionary.Add(soundData.AudioClipName, subclips);
        }
    }

    //===========================================================================
    private AudioClip CreateSubClip(AudioClip originalClip, float startTime, float duration)
    {
        int sampleStart = (int)(startTime * originalClip.frequency);
        int sampleLength = (int)(duration * originalClip.frequency);

        // Create and fill sub-clip
        AudioClip subClip = AudioClip.Create(originalClip.name + "_subClip_" + startTime, sampleLength, originalClip.channels, originalClip.frequency, false);
        float[] data = new float[sampleLength * originalClip.channels];
        originalClip.GetData(data, sampleStart);
        subClip.SetData(data, 0);

        return subClip;
    }

    private List<AudioClip> CreateSubClips(SO_AudioClipComposite soundData)
    {
        List<AudioClip> subclips = new List<AudioClip>();
        float singleSoundDuration = soundData.AudioClip.length / soundData.SoundCount;

        for (int i = 0; i < soundData.SoundCount; i++)
        {
            float startTime = i * singleSoundDuration;
            AudioClip subClip = CreateSubClip(soundData.AudioClip, startTime, singleSoundDuration);
            subclips.Add(subClip);
        }

        return subclips;
    }

    //===========================================================================
    public void PlaySound(string soundTags)
    {
        foreach (SO_AudioClip clip in audioList.AudioList)
        {
            if (clip.AudioClipName != soundTags)
                continue;

            sfxSource.PlayOneShot(clip.AudioClip);
        }
    }

    public void PlaySoundComposite(string soundTags)
    {
        if (audioSubclipDictionary.ContainsKey(soundTags) == false)
            return;

        List<AudioClip> subclips = audioSubclipDictionary[soundTags];
        int index = Random.Range(0, subclips.Count);
        sfxSource.PlayOneShot(subclips[index]);
    }
}