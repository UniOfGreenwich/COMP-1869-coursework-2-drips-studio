using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// General component that plays a random <see cref="AudioEvent"/> from a list when triggered.
/// Can be used from Unity Events or via script to play sound effects in a scene.
/// </summary>
public class RandomSoundEffectTrigger : MonoBehaviour
{
    /// <summary>List of audio events to randomly pick from when triggered.</summary>
    [Tooltip("List of audio events to choose from randomly.")]
    public List<AudioEvent> audioEvents = new();

    [Tooltip("Audio service used for playback. If not set, the global AudioManager is used.")]
    [SerializeField]
    private MonoBehaviour audioServiceSource;

    private void Awake()
    {
        ValidateAudioService();
    }

    private void Start()
    {
        if (audioServiceSource == null)
        {
            audioServiceSource = FindObjectOfType<AudioManager>();
        }
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
    /// Plays a random audio event from this GameObject's position.
    /// </summary>
    public void Play()
    {
        PlayAtPosition(transform.position);
    }

    /// <summary>
    /// Plays a random audio event from a specific world position.
    /// </summary>
    /// <param name="position">World position to play the sound.</param>
    public void PlayAtPosition(Vector3 position)
    {
        if (audioEvents == null || audioEvents.Count == 0)
        {
            Debug.LogWarning($"{name}: No audio events assigned.");
            return;
        }

        AudioEvent selectedEvent = audioEvents[Random.Range(0, audioEvents.Count)];

        if (selectedEvent != null)
        {
            AudioService.PlayOneShot(selectedEvent, position);
        }
    }
}
