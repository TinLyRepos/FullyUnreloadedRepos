using NUnit.Framework;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonobehaviour<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        // Load components
        musicAudioSource = GetComponent<AudioSource>();

        // Start with music off
        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        // Check if volume levels have been saved in playerprefs - if so retrieve and set them
        if (PlayerPrefs.HasKey("musicVolume"))
            musicVolume = PlayerPrefs.GetInt("musicVolume");

        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }

    /// Play music for room routine
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        // if fade out routine already running then stop it
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        // if fade in routine already running then stop it
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        // If the music track has changed then play new music track
        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }

    /// Fade out music routine
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    /// Fade in music routine
    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        // Set clip & play
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    //===========================================================================
    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
    {
        Assert.IsNotNull(musicTrack);
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    public void IncreaseMusicVolume()
    {
        int maxMusicVolume = 20;

        if (musicVolume >= maxMusicVolume) return;

        musicVolume += 1;

        SetMusicVolume(musicVolume);
    }

    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;

        musicVolume -= 1;

        SetMusicVolume(musicVolume);
    }

    public void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(musicVolume));
        }
    }
}