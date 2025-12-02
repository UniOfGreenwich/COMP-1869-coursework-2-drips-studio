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

        // Spawn ticket under canvas
        GameObject newTicket = Instantiate(ticketPrefab, mainCanvas.transform);

        RectTransform rect = newTicket.GetComponent<RectTransform>();
        if (rect == null)
            rect = newTicket.AddComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(145f, -290f);
        rect.localScale = Vector3.one;

        // Get TicketInstance and assign this manager directly (THE FIX)
        TicketInstance ticket = newTicket.GetComponent<TicketInstance>();
        if (ticket != null)
        {
            ticket.tm = this;        // <-- FIX: direct assignment
            ticket.GenerateOrder();
        }
        else
        {
            Debug.LogError("Spawned ticket has no TicketInstance component!");
        }

        currentTickets++;
        Debug.Log("Spawned new ticket in center of screen!");
    }

    public void RemoveTicket(GameObject ticketObj)
    {
        currentTickets = Mathf.Max(0, currentTickets - 1);
        Destroy(ticketObj);
        Debug.Log("Ticket removed");
    }
    public void ResetTickets()
    {
        if (mainCanvas != null)
        {
            foreach (Transform child in mainCanvas.transform)
            {
                if (child.GetComponent<TicketInstance>())
                {
                    Destroy(child.gameObject);
                }
            }
        }
        currentTickets = 0;
    }
}
