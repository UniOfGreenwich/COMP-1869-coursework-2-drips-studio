using UnityEngine;

public class TicketManager : MonoBehaviour
{
    [Header("Ticket Settings")]
    public GameObject ticketPrefab;   // Assign your Ticket prefab here
    public int maxTickets = 1;

    public int currentTickets = 0;
    public Canvas mainCanvas;

    public void SpawnTicket()
    {
        if (ticketPrefab == null)
        {
            Debug.LogError("No ticket prefab assigned!");
            return;
        }

        if (mainCanvas == null)
        {
            Debug.LogError("No Canvas found!");
            return;
        }

        if (currentTickets >= maxTickets)
        {
            Debug.Log("Maximum number of tickets reached!");
            return;
        }

        // Spawn ticket in the middle of the screen
        GameObject newTicket = Instantiate(ticketPrefab, mainCanvas.transform);

        RectTransform rect = newTicket.GetComponent<RectTransform>();
        if (rect == null)
            rect = newTicket.AddComponent<RectTransform>();

        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;

        // Generate random order
        TicketInstance ticket = newTicket.GetComponent<TicketInstance>();
        if (ticket != null)
            ticket.GenerateOrder();

        currentTickets++;
        Debug.Log("Spawned new ticket in center of screen!");
    }

    public void RemoveTicket(GameObject ticket)
    {
        Destroy(ticket);
        currentTickets--;
        Debug.Log("Ticket removed");
    }
}
