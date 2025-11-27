using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Controls audio layers used in a music track, allowing for dynamic fading in/out.
/// </summary>
/// <summary>
/// Component that manages multiple music layers using an AudioMixer for smooth
/// fading between them.
/// </summary>
public class MusicLayerController : MonoBehaviour {
    [System.Serializable]
    public class MusicLayer {
        /// <summary>
        /// Name of this music layer.
        /// </summary>
        [Tooltip("Name of this music layer.")]
        public string name;

        /// <summary>
        /// AudioSource associated with this layer.
        /// </summary>
        [Tooltip("AudioSource associated with this layer.")]
        public AudioSource source;

        /// <summary>
        /// Exposed parameter name in the AudioMixer (e.g., 'MusicLayer1Vol').
        /// </summary>
        [Tooltip("Exposed parameter name in the AudioMixer (e.g., 'MusicLayer1Vol').")]
        public string mixerParameter;

        /// <summary>
        /// Should this layer be active when the level starts?
        /// </summary>
        [Tooltip("Should this layer be active when the level starts?")]
        public bool startActive = false;

        /// <summary>
        /// Time it takes to fade this layer in or out.
        /// </summary>
        [Tooltip("Time it takes to fade this layer in or out.")]
        public float fadeDuration = 1f;
    }

    [Header("Music Layers")]
    [Tooltip("List of music layers controlled by this component.")]
    public List<MusicLayer> layers;

    [Header("Audio Mixer")]
    [Tooltip("Reference to the AudioMixer controlling volume.")]
    public AudioMixer audioMixer;

    /// <summary>
    /// Quick lookup dictionary for music layers by name.
    /// </summary>
    private Dictionary<string, MusicLayer> layerLookup = new();
    private readonly Dictionary<string, Coroutine> fadeRoutines = new();

    private void Start() {
        if (audioMixer == null) {
            Debug.LogWarning($"{name}: No AudioMixer assigned to MusicLayerController.", this);
            return;
        }

        layerLookup.Clear();
        foreach (var layer in layers) {
            if (!string.IsNullOrEmpty(layer.name) && !layerLookup.ContainsKey(layer.name))
                layerLookup[layer.name] = layer;

            SetMixerVolume(layer.mixerParameter, layer.startActive ? 1f : 0f);
            if (!audioMixer.GetFloat(layer.mixerParameter, out _))
                Debug.LogWarning($"{name}: Mixer parameter '{layer.mixerParameter}' not found in AudioMixer.", this);
            if (layer.startActive) {
                layer.source.Play();
            } else {
                layer.source.Pause();
            }
        }
    }

    /// <summary>
    /// Fades in a music layer by name.
    /// </summary>
    /// <param name="layerName">Name of the layer to fade in.</param>
    /// <param name="targetVolume">Target mixer volume from 0-1.</param>
    /// <param name="durationOverride">Optional duration for the fade. Use -1 to use the layer's default.</param>
    public void FadeIn(string layerName, float targetVolume = 1f, float durationOverride = -1f)
    {
        var layer = layers.Find(l => l.name == layerName);
        if (layer != null && audioMixer != null)
        {
            if (fadeRoutines.TryGetValue(layer.name, out var routine))
                StopCoroutine(routine);

            // Ensure starting volume is 0 before play
            SetMixerVolume(layer.mixerParameter, 0f);
            fadeRoutines[layer.name] = StartCoroutine(FadeLayer(layer, true, targetVolume, durationOverride));
        }
    }

    /// <summary>
    /// Fades out a music layer by name.
    /// </summary>
    /// <param name="layerName">Name of the layer to fade out.</param>
    /// <param name="durationOverride">Optional duration for the fade. Use -1 to use the layer's default.</param>
    public void FadeOut(string layerName, float durationOverride = -1f) {
        var layer = layers.Find(l => l.name == layerName);
        if (layer != null && audioMixer != null) {
            if (fadeRoutines.TryGetValue(layer.name, out var routine))
                StopCoroutine(routine);
            fadeRoutines[layer.name] = StartCoroutine(FadeLayer(layer, false, 1f, durationOverride));
        }

    }

    /// <summary>
    /// Cross fades from one layer to another using each layer's fade duration.
    /// </summary>
    /// <param name="fromLayer">Layer to fade out.</param>
    /// <param name="toLayer">Layer to fade in.</param>
    /// <param name="targetVolume">Target volume for the layer being faded in.</param>
    /// <param name="durationOverride">Optional duration for the fade. Use -1 to use the layer's default.</param>

    public void CrossFade(string fromLayer, string toLayer, float targetVolume = 1f, float durationOverride = -1f) {
        FadeOut(fromLayer, durationOverride);
        FadeIn(toLayer, targetVolume, durationOverride);

    }

    /// <summary>
    /// Coroutine for fading a music layer in or out using the AudioMixer.
    /// </summary>
    private IEnumerator FadeLayer(MusicLayer layer, bool fadeIn, float targetVolume = 1f, float durationOverride = -1f) {
        float duration = durationOverride >= 0f ? durationOverride : layer.fadeDuration;
        float startVol = GetMixerVolume(layer.mixerParameter);
        float endVol = fadeIn ? targetVolume : 0f;

        if (fadeIn && !layer.source.isPlaying)
            layer.source.Play();

        float t = 0;
        while (t < duration) {
            float newVol = Mathf.Lerp(startVol, endVol, t / duration);
            SetMixerVolume(layer.mixerParameter, newVol);
            t += Time.deltaTime;
            yield return null;
        }

        SetMixerVolume(layer.mixerParameter, endVol);
        if (!fadeIn) layer.source.Pause();
        fadeRoutines.Remove(layer.name);
    }


    /// <summary>
    /// Sets the volume of a mixer parameter from 0.0 to 1.0, converting to decibels.
    /// </summary>
    private void SetMixerVolume(string parameter, float volume) {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, db);
    }

    /// <summary>
    /// Gets the current volume of a mixer parameter as a linear value.
    /// </summary>
    private float GetMixerVolume(string parameter) {
        return audioMixer.GetFloat(parameter, out float db)
            ? Mathf.Pow(10f, db / 20f)
            : 0f;
    }
}
