using UnityEngine;

/// <summary>
/// General component that plays a single <see cref="AudioEvent"/> when triggered.
/// Can be used from Unity Events or via script to play sound effects in a scene.
/// </summary>
public class SoundEffectTrigger : MonoBehaviour {
    /// <summary>Audio event to play when triggered.</summary>
    [Tooltip("The audio event to play when triggered.")]
    public AudioEvent audioEvent;

    [Tooltip("Audio service used for playback. If not set, the global AudioManager is used.")]
    [SerializeField]
    private MonoBehaviour audioServiceSource;

    private void Awake()
    {
        ValidateAudioService();
    }

    private void Start()
    {
        audioServiceSource = FindObjectOfType<AudioManager>();
    }

    private void OnValidate()
    {
        ValidateAudioService();
    }

    private void ValidateAudioService()
    {
        if (audioServiceSource != null && audioServiceSource is not IAudioService)
        {
            Debug.LogWarning($"{name}: Assigned audio service does not implement IAudioService.", this);
            audioServiceSource = null;
        }
    }

    /// <summary>Resolved audio service for playback.</summary>
    private IAudioService AudioService =>
        (audioServiceSource as IAudioService) ?? AudioManager.Instance;

    /// <summary>
    /// Plays the configured audio event from this GameObject's position.
    /// </summary>
    public void Play()
    {
        PlayAtPosition(transform.position);
    }

    /// <summary>
    /// Plays the configured audio event from a specific world position.
    /// </summary>
    /// <param name="position">World position to play the sound.</param>
    public void PlayAtPosition(Vector3 position)
    {
        if (audioEvent != null)
        {
            AudioService.PlayOneShot(audioEvent, position);
        }
    }
}
