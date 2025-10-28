using System.Collections.Generic;
using UnityEngine;

public class ControlAudioManager : MonoBehaviour
{
    public GameObject audioObject;

    public enum SFXCat { normalArrow, fireArrow, electricArrow, electricPulse, electricCharge, sonicBoom }

    public AudioClip[] normalArrow;
    public AudioClip[] fireArrow;
    public AudioClip[] electricArrow;
    public AudioClip[] electricPulse;
    public AudioClip[] electricCharge;
    public AudioClip[] sonicBoom;

    private List<GameObject> activeAudioObjects = new List<GameObject>();

    public void AudioTrigger(SFXCat audioType, Vector3 audioPosition, float volume)
    {
        GameObject newAudio = Instantiate(audioObject, audioPosition, Quaternion.identity);
        ControlAudio ca = newAudio.GetComponent<ControlAudio>();

        switch (audioType)
        {
            case SFXCat.normalArrow:
                ca.clip = normalArrow[Random.Range(0, normalArrow.Length)];
                break;
            case SFXCat.fireArrow:
                ca.clip = fireArrow[Random.Range(0, fireArrow.Length)];
                break;
            case SFXCat.electricArrow:
                ca.clip = electricArrow[Random.Range(0, electricArrow.Length)];
                break;
            case SFXCat.electricPulse:
                ca.clip = electricPulse[Random.Range(0, electricPulse.Length)];
                break;
            case SFXCat.electricCharge:
                ca.clip = electricCharge[Random.Range(0, electricCharge.Length)];
                break;
            case SFXCat.sonicBoom:
                ca.clip = sonicBoom[Random.Range(0, sonicBoom.Length)];
                break;
        }

        ca.audioType = audioType;
        ca.volume = volume;
        ca.StartAudio();

        activeAudioObjects.Add(newAudio);
    }

    public void StopAudio(SFXCat? typeToStop = null)
    {
        foreach (GameObject audioObj in activeAudioObjects)
        {
            if (audioObj != null)
            {
                ControlAudio ca = audioObj.GetComponent<ControlAudio>();
                if (ca != null && (typeToStop == null || ca.audioType == typeToStop))
                {
                    ca.StopAudio();
                }
            }
        }

        activeAudioObjects.RemoveAll(obj => obj == null);
    }
}
