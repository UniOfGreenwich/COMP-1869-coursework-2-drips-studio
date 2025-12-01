using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    [Header("UI")]
    public Button serveButton;

    [Header("References")]
    [SerializeField] private TicketManager tm;

    public bool playerInside = false;
    public bool customerInside = false;
    private CustomerController customerController;
    private RandomSoundEffectTrigger trigger;
    private InteractSquishAnimation squish;

    private void Awake()
    {
        // Find TicketManager if not assigned
        if (tm == null)
            tm = FindAnyObjectByType<TicketManager>();

        trigger = GetComponent<RandomSoundEffectTrigger>();
        squish = GetComponent<InteractSquishAnimation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;

        if (other.CompareTag("Customer"))
        {
            customerInside = true;

            // Get the CustomerController on this object
            customerController = other.GetComponent<CustomerController>();

            if (customerController == null)
                Debug.LogError("[InteractableObject]: CustomerController NOT found on " + other.name);
            else
                Debug.Log("[InteractableObject]: CustomerController detected on " + customerController.gameObject.name);
        }

        UpdateButtonState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;

        if (other.CompareTag("Customer"))
        {
            customerInside = false;
            customerController = null;
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (serveButton != null)
            serveButton.gameObject.SetActive(playerInside && customerInside);
    }

    public void OnServeButtonPressed()
    {
        if (customerController == null)
        {
            Debug.LogError("[InteractableObject]: Serve pressed but no CustomerController found!");
            return;
        }

        if (tm.currentTickets >= tm.maxTickets)
        {
            Debug.Log("[InteractableObject]: Cannot serve — Max tickets reached");
            return;
        }

        // Serve the customer
        Debug.Log("[InteractableObject]: Serving customer, spawning ticket");
        customerController.hasBeenServed = true;
        tm.SpawnTicket();
        trigger.Play();
        squish.squish = true;
    }
}
