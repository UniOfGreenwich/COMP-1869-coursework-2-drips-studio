using UnityEngine;

public class DoorbellChime : MonoBehaviour
{
    public SoundEffectTrigger trigger;

    private void Start()
    {
        trigger = GetComponent<SoundEffectTrigger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Customer")) trigger.Play();

    }
}
