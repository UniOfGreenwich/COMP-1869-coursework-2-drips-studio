using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeInMusic : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;
    [Range(0f, 1f)] public float targetVolume = 0.1f;
    public bool playOnStart = false;

    [Header("Audio Sources")]
    public List<AudioSource> introSources = new List<AudioSource>();
    public List<AudioSource> loopSources = new List<AudioSource>();

    private int currentTrackIndex = -1;
    private Coroutine introFadeCoroutine;
    private Coroutine loopFadeCoroutine;
    private Coroutine loopStartCoroutine;

    void Start()
    {
        if (playOnStart && introSources.Count > 0 && loopSources.Count > 0)
        {
            PlayTrack(0);
        }
    }

    public void PlayTrack(int newIndex)
    {
        if (!IsValidIndex(newIndex)) return;

        // Stop previous coroutines
        StopCoroutineIfRunning(ref introFadeCoroutine);
        StopCoroutineIfRunning(ref loopFadeCoroutine);
        StopCoroutineIfRunning(ref loopStartCoroutine);

        // If another track is playing, fade it out
        if (currentTrackIndex != -1 && currentTrackIndex != newIndex)
        {
            AudioSource oldIntro = introSources[currentTrackIndex];
            AudioSource oldLoop = loopSources[currentTrackIndex];
            StartCoroutine(FadeOutAndStop(oldIntro, fadeOutDuration));
            StartCoroutine(FadeOutAndStop(oldLoop, fadeOutDuration));
        }

        // Start new track
        currentTrackIndex = newIndex;

        AudioSource intro = introSources[newIndex];
        AudioSource loop = loopSources[newIndex];

        // Prepare sources
        intro.volume = 0f;
        intro.loop = false;
        loop.volume = 0f;
        loop.loop = true;

        // Play intro
        intro.Play();
        introFadeCoroutine = StartCoroutine(FadeIn(intro, fadeInDuration));

        // Schedule loop to start and fade in during intro
        loopStartCoroutine = StartCoroutine(FadeInLoopDuringIntro(intro, loop));
    }

    public void FadeOutCurrent()
    {
        if (!IsValidIndex(currentTrackIndex)) return;

        AudioSource intro = introSources[currentTrackIndex];
        AudioSource loop = loopSources[currentTrackIndex];

        StopCoroutineIfRunning(ref introFadeCoroutine);
        StopCoroutineIfRunning(ref loopFadeCoroutine);
        StopCoroutineIfRunning(ref loopStartCoroutine);

        StartCoroutine(FadeOutAndStop(intro, fadeOutDuration));
        StartCoroutine(FadeOutAndStop(loop, fadeOutDuration));
        currentTrackIndex = -1;
    }

    private IEnumerator FadeInLoopDuringIntro(AudioSource intro, AudioSource loop)
    {
        double startTime = AudioSettings.dspTime + intro.clip.length;
        loop.PlayScheduled(startTime);

        float time = 0f;
        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            loop.volume = Mathf.Lerp(0f, targetVolume, time / fadeInDuration);
            yield return null;
        }

        loop.volume = targetVolume;
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        float time = 0f;
        source.volume = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    private IEnumerator FadeOutAndStop(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    private void StopCoroutineIfRunning(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < introSources.Count && index < loopSources.Count;
    }
}
