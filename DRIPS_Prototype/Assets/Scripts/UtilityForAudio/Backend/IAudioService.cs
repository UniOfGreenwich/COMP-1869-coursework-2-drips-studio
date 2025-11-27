using UnityEngine;

/// <summary>
/// Interface defining common audio playback functionality.
/// </summary>
public interface IAudioService
{
    /// <summary>
    /// Plays an <see cref="AudioEvent"/> at the given world position.
    /// </summary>
    /// <param name="audioEvent">Audio event describing clip settings.</param>
    /// <param name="position">World position for playback.</param>
    void PlayOneShot(AudioEvent audioEvent, Vector3 position);
}
