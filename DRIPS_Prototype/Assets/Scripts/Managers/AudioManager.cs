using UnityEngine;
using UnityEngine.Audio;


/// <summary>
/// Global audio manager that implements <see cref="IAudioService"/> and provides
/// pooled one-shot audio playback.
/// </summary>
public class AudioManager : MonoBehaviour, IAudioService {
    /// <summary>
    /// Singleton instance of the AudioManager.
    /// </summary>
    public static AudioManager Instance { get; private set; }

    /// <summary>
    /// Main audio mixer for volume control and effects.
    /// </summary>
    [Header("Audio Mixer")]
    [Tooltip("Main audio mixer for volume control and effects.")]
    public AudioMixer mixer;

    [Header("One Shot Pool")]
    [Tooltip("Number of AudioSources preallocated for one-shot playback.")]
    [SerializeField]
    private int poolSize = 10;

    private ObjectPool<AudioSource> oneShotPool;
    private AudioSource pooledSourcePrefab;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (mixer == null)
            Debug.LogWarning($"{name}: AudioMixer not assigned to AudioManager.", this);

        GameObject prefabGO = new GameObject("PooledAudioSource");
        prefabGO.transform.SetParent(transform);
        pooledSourcePrefab = prefabGO.AddComponent<AudioSource>();
        pooledSourcePrefab.gameObject.SetActive(false);

        oneShotPool = new ObjectPool<AudioSource>(pooledSourcePrefab, poolSize, transform, poolSize);
    }

    /// <summary>
    /// Plays a one-shot sound at a specific world position.
    /// </summary>
    /// <param name="audioEvent">Event describing the clip and playback settings.</param>
    /// <param name="position">World position from which the sound should play.</param>
    public void PlayOneShot(AudioEvent audioEvent, Vector3 position) {
        if (audioEvent == null || audioEvent.clip == null)
            return;

        AudioSource source = oneShotPool.Get();
        source.transform.position = position;
        source.clip = audioEvent.clip;
        source.volume = audioEvent.volume;
        source.pitch = audioEvent.pitch;
        source.loop = audioEvent.loop;
        source.priority = audioEvent.priority;
        source.spatialBlend = audioEvent.spatial ? 1f : 0f;
        source.rolloffMode = audioEvent.rolloff;
        source.minDistance = audioEvent.minDistance;
        source.maxDistance = audioEvent.maxDistance;
        /*source.outputAudioMixerGroup = audioEvent.mixerGroupOverride != null
            ? audioEvent.mixerGroupOverride
            : null;*/
        source.Play();

        if (!audioEvent.loop)
            StartCoroutine(ReturnAfterPlay(source, audioEvent.clip.length / audioEvent.pitch));
    }

    private System.Collections.IEnumerator ReturnAfterPlay(AudioSource source, float time) {
        yield return new WaitForSeconds(time);
        source.clip = null;
        source.loop = false;
        source.priority = 128;
        oneShotPool.Release(source);
    }

    /// <summary>
    /// Stops a looping AudioSource started via <see cref="PlayOneShot"/> and
    /// returns it to the internal pool.
    /// </summary>
    /// <param name="source">AudioSource that should stop looping.</param>
    public void StopLoop(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.clip = null;
        source.loop = false;
        source.priority = 128;
        oneShotPool.Release(source);
    }
}
