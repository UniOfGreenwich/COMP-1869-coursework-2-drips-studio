using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Music Event Timeline")]
public class MusicEventTimeline : ScriptableObject {
    /// <summary>Should the timeline loop when reaching the end.</summary>
    [Tooltip("Should the timeline loop when reaching the end?")]
    public bool loop = false;

    [System.Serializable]
    public class MusicLayerEvent {
        [Tooltip("Time in seconds at which this event occurs.")]
        public float time;
        [Tooltip("Name of the layer to activate or deactivate.")]
        public string layerName;
        [Tooltip("Whether to activate (true) or deactivate (false) the layer.")]
        public bool activate;

        [Range(0f, 1f)]
        [Tooltip("Target volume for this layer (only used if activate is true).")]
        public float targetVolume = 1f;
    }


    [System.Serializable]
    public class SFXEvent {
        [Tooltip("Time in seconds at which the sound effect plays.")]
        public float time;
        [Tooltip("Audio event describing the clip and settings.")]
        public AudioEvent audioEvent;

        [Tooltip("The GameObject from which the sound will play.")]
        public GameObject sourceObject;
    }


    [System.Serializable]
    public class MusicCrossFadeEvent {
        [Tooltip("Time in seconds at which the cross fade begins.")]
        public float time;
        [Tooltip("Layer that will fade out.")]
        public string fromLayer;
        [Tooltip("Layer that will fade in.")]
        public string toLayer;
        [Range(0f,1f)]
        [Tooltip("Target volume for the layer being faded in.")]
        public float targetVolume = 1f;

        [Tooltip("Duration of the cross fade in seconds.")]
        public float duration = 1f;
    }

    [Header("Layer Events")]
    [Tooltip("Ordered list of layer activation events.")]
    public List<MusicLayerEvent> events = new();

    [Header("Cross Fades")]
    [Tooltip("Ordered list of cross fade events between layers.")]
    public List<MusicCrossFadeEvent> crossFadeEvents = new();

    [Header("Sound Effects")]
    [Tooltip("Timed sound effects to trigger during playback.")]
    public List<SFXEvent> sfxEvents = new();
}
