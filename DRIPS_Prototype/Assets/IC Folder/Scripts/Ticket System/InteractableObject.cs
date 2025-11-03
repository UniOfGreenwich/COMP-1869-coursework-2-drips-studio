using Unity.VisualScripting;
using UnityEngine;

public class InteractableObjects : MonoBehaviour
{
    [SerializeField] private TicketManager ticketManager;

    private void Start()
    {
        ticketManager = FindAnyObjectByType<TicketManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[ InteractObjects ]: Player Is Touching Coffee Machine Trigger");
            Debug.Log("Spawning Ticket!");
            ticketManager.SpawnTicket();
        }
    }
}
