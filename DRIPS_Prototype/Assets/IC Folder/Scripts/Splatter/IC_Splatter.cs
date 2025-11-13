using UnityEngine;

public class IC_Splatter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) Destroy(gameObject);
    }
}
