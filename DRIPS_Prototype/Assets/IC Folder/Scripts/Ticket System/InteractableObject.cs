using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    public Button serveButton;

    private bool playerInside = false;
    private bool customerInside = false;
    private CustomerController customerController;
    [SerializeField] TicketManager tm;

    private void Start()
    {
        tm = FindAnyObjectByType<TicketManager>();
    }
    private void OnEnable()
    {
        StartCoroutine(WaitForButton());
    }

    private IEnumerator WaitForButton()
    {
        yield return new WaitUntil(() => serveButton != null && serveButton.gameObject.activeInHierarchy);

        serveButton.gameObject.SetActive(false);
        serveButton.onClick.RemoveAllListeners();
        serveButton.onClick.AddListener(OnServeButtonPressed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;

        if (other.CompareTag("Customer"))
        {
            customerInside = true;
            customerController = other.GetComponent<CustomerController>();
        }

        if (playerInside && customerInside)
            serveButton.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;

        if (other.CompareTag("Customer"))
            customerInside = false;

        if (!playerInside || !customerInside)
            serveButton.gameObject.SetActive(false);
    }

    public void OnServeButtonPressed()
    {
        if (tm.currentTickets == tm.maxTickets)
        {
            Debug.Log("Cannot Serve: Max Tickets Reached");
        }
        else
        {
            Debug.Log("should spawn ticket and customer should sit down");
            customerController.hasBeenServed = true;
            tm.SpawnTicket();
        }
    }
}
