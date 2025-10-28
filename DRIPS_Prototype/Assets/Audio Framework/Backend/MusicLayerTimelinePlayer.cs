using UnityEngine;

/// <summary>
/// Component that plays a <see cref="MusicEventTimeline"/> by triggering events
/// over time. Supports play, stop, pause and optional looping.
/// </summary>
public class MusicEventTimelinePlayer : MonoBehaviour {
    /// <summary>Controller responsible for handling music layer fades.</summary>
    [Tooltip("Reference to the MusicLayerController that controls the music layers.")]
    public MusicLayerController controller;

    /// <summary>Timeline asset defining the sequence of events to play.</summary>
    [Tooltip("Timeline asset defining music and SFX layer events.")]
    public MusicEventTimeline timeline;

    /// <summary>Optional service used for playing sound effects.</summary>
    [Tooltip("Service used for one-shot playback. Defaults to AudioManager.Instance if not assigned.")]
    public IAudioService audioService;


    private float timer = 0f;
    private int currentMusicEventIndex = 0;
    private int currentCrossFadeEventIndex = 0;
    private int currentSFXEventIndex = 0;

    private bool isPlaying = false;
    private bool isPaused = false;
    private float timelineDuration = 0f;

    /// <summary>
    /// Initializes cached timeline duration when a timeline is assigned.
    /// </summary>
    private void Awake() {
        if (timeline != null)
        {
            SortTimeline();
            CalculateDuration();
        }
    }


    /// <summary>
    /// Ensures the timeline event lists remain sorted when edited in the inspector.
    /// </summary>
    private void OnValidate() {
        if (timeline != null)
            SortTimeline();
    }

    private void Update() {
        if (!isPlaying || isPaused)

            return;
        if (timeline == null || controller == null)
        {
            Debug.LogWarning($"{name}: Timeline player missing controller or timeline.", this);
            isPlaying = false;
            return;

        }

        timer += Time.deltaTime;

        // Handle music layer events
        while (currentMusicEventIndex < timeline.events.Count &&
               timer >= timeline.events[currentMusicEventIndex].time) {
            var e = timeline.events[currentMusicEventIndex];
            if (e.activate) controller.FadeIn(e.layerName, e.targetVolume);
            else controller.FadeOut(e.layerName);
            currentMusicEventIndex++;
        }

        // Handle cross fade events
        while (currentCrossFadeEventIndex < timeline.crossFadeEvents.Count &&
               timer >= timeline.crossFadeEvents[currentCrossFadeEventIndex].time) {
            var cf = timeline.crossFadeEvents[currentCrossFadeEventIndex];
            controller.CrossFade(cf.fromLayer, cf.toLayer, cf.targetVolume, cf.duration);
            currentCrossFadeEventIndex++;
        }

        // Handle SFX events
        while (currentSFXEventIndex < timeline.sfxEvents.Count &&
               timer >= timeline.sfxEvents[currentSFXEventIndex].time) {
            var sfx = timeline.sfxEvents[currentSFXEventIndex];
            if (sfx.audioEvent != null && sfx.sourceObject != null) {
                PlayAudioEvent(sfx.audioEvent, sfx.sourceObject);
            }
            currentSFXEventIndex++;
        }

        // Looping
        if (timeline.loop && timer > timelineDuration) {
            ResetIndices();
        }
    }

    /// <summary>
    /// Begins playback of the timeline from the start.
    /// </summary>
    public void Play() {
        if (timeline == null || controller == null)
        {
            Debug.LogWarning($"{name}: Cannot play timeline - missing controller or timeline.", this);
            return;
        }
        timer = 0f;
        isPaused = false;
        isPlaying = true;
        SortTimeline();
        CalculateDuration();
        ResetIndices();
    }

    /// <summary>
    /// Stops playback and resets the internal timer and event indices.
    /// </summary>
    public void Stop() {
        isPlaying = false;
        isPaused = false;
        timer = 0f;
        ResetIndices();
    }

    /// <summary>
    /// Pauses playback without resetting indices.
    /// </summary>
    public void Pause() {
        isPaused = true;
    }

    /// <summary>
    /// Resumes playback after a pause.
    /// </summary>
    public void Resume() {
        isPaused = false;
    }

    /// <summary>
    /// Sorts all timeline event lists by their time value.
    /// </summary>
    private void SortTimeline() {
        if (timeline == null) return;
        timeline.events.Sort((a, b) => a.time.CompareTo(b.time));
        timeline.crossFadeEvents.Sort((a, b) => a.time.CompareTo(b.time));
        timeline.sfxEvents.Sort((a, b) => a.time.CompareTo(b.time));
    }

    /// <summary>
    /// Calculates the total length of the assigned timeline.
    /// </summary>
    private void CalculateDuration() {
        float max = 0f;
        if (timeline.events.Count > 0)
            max = Mathf.Max(max, timeline.events[^1].time);
        if (timeline.crossFadeEvents.Count > 0)
            max = Mathf.Max(max, timeline.crossFadeEvents[^1].time);
        if (timeline.sfxEvents.Count > 0)
            max = Mathf.Max(max, timeline.sfxEvents[^1].time);
        timelineDuration = max;
    }

    /// <summary>
    /// Resets event indices to the beginning of the timeline.
    /// </summary>
    private void ResetIndices() {
        currentMusicEventIndex = 0;
        currentCrossFadeEventIndex = 0;
        currentSFXEventIndex = 0;
        timer = 0f;
    }

    /// <summary>
    /// Plays the provided audio event from the given object using the configured service.
    /// </summary>
    /// <param name="audioEvent">Event describing the audio clip and settings.</param>
    /// <param name="sourceObject">GameObject from which to play the sound.</param>
    private void PlayAudioEvent(AudioEvent audioEvent, GameObject sourceObject) {
        if (audioEvent == null || sourceObject == null)
            return;

        IAudioService service = audioService != null ? audioService : AudioManager.Instance;
        if (service == null) {
            Debug.LogWarning("No audio service available to play SFX.");
            return;
        }

        service.PlayOneShot(audioEvent, sourceObject.transform.position);
    }
}
