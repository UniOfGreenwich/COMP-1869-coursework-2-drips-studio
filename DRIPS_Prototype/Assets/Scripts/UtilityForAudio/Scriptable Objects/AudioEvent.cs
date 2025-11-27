using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Event")]
/// <summary>
/// Scriptable object describing how an individual audio clip should be played.
/// </summary>
public class AudioEvent : ScriptableObject {
    /// <summary>Audio clip to be played.</summary>
    [Tooltip("Clip asset to play.")]
    public AudioClip clip;

    /// <summary>Volume multiplier for playback.</summary>
    [Range(0f, 1f)]
    [Tooltip("Volume multiplier for playback.")]
    public float volume = 1f;
    /// <summary>Pitch multiplier for playback.</summary>
    [Range(0.1f, 3f)]
    [Tooltip("Pitch multiplier for playback.")]
    public float pitch = 1f;

    /// <summary>Enable 3D spatialisation for this sound.</summary>
    [Tooltip("Enable 3D spatialisation for this sound.")]
    public bool spatial = false;
    /// <summary>Rolloff mode when spatialised.</summary>
    [Tooltip("Rolloff mode when spatialised.")]
    public AudioRolloffMode rolloff = AudioRolloffMode.Logarithmic;
    /// <summary>Minimum distance for audibility when spatialised.</summary>
    [Tooltip("Minimum distance for audibility when spatialised.")]
    public float minDistance = 1f;
    /// <summary>Maximum distance for audibility when spatialised.</summary>
    [Tooltip("Maximum distance for audibility when spatialised.")]
    public float maxDistance = 20f;

    /// <summary>Should the clip loop when played?</summary>
    [Tooltip("Should the clip loop when played?")]
    public bool loop = false;

    /// <summary>Playback priority (0 = highest).</summary>
    [Tooltip("Playback priority (0 = highest).")]
    [Range(0, 256)]
    public int priority = 128;

    /// <summary>Optional override mixer group for this sound. Leave null to use default.</summary>
    [Tooltip("Optional override mixer group for this sound. Leave null to use default.")]
    public AudioMixerGroup mixerGroupOverride;
}
