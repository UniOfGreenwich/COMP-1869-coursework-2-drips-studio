using UnityEngine;

public class ControlAudio : MonoBehaviour
{
    private AudioSource source;

    public AudioClip clip;
    public float volume = 1f;

    public ControlAudioManager.SFXCat audioType;

    public void StartAudio()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
        {
            Debug.LogError("Missing AudioSource on " + gameObject.name);
            return;
        }

        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f; // 3D spatial audio
        source.Play();
    }

    public void StopAudio()
    {
        if (source != null)
        {
            source.Stop();
        }

        Destroy(gameObject, 0.1f);
    }

    void Update()
    {
        if (source != null && !source.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
