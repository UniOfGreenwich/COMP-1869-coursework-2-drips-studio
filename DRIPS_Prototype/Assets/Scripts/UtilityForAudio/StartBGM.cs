using UnityEngine;

public class StartBGM : MonoBehaviour
{
    public SoundEffectTrigger trigger;

    private void Start()
    {
        trigger = GetComponent<SoundEffectTrigger>();
        trigger.Play();
    }
}
