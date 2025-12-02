using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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
        {
            playerInside = true;

            // Access the player's NavMeshAgent
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.updateRotation = false; // disable rotation override
            }

            // Force rotation to your predefined rotation
            other.transform.root.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

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
        {
            playerInside = false;

            // Restore normal NavMeshAgent rotation
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.updateRotation = true;
            }
        }

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
            Debug.Log("[InteractactableObject]: Cannot serve — Max tickets reached");
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
