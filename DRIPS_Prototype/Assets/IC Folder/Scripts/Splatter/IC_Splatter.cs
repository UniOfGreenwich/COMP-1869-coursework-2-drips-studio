using UnityEngine;

public class IC_Splatter : MonoBehaviour
{
    public SoundEffectTrigger trigger;

    private void OnTriggerEnter(Collider other)
    {
        trigger.Play();
        if (other.gameObject.CompareTag("Player")) Destroy(gameObject, 1);
    }
}
