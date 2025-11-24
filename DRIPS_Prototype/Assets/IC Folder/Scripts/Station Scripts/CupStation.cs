using UnityEngine;
using UnityEngine.UI;
using static DrinkIngredientsEnum;

public class CupStation : MonoBehaviour
{
    private bool playerInside = false;
    private float lastToggleTime = 0f;

    [SerializeField] private float toggleCooldown = 1f;
    public Button interactButton;

    private void Awake()
    {
        if (interactButton == null)
            Debug.LogError("CupStation: interactButton reference missing!");
    }

    private void Start()
    {
        // Start with button hidden
        interactButton.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        // Show button only if cooldown ready
        if (Time.time - lastToggleTime >= toggleCooldown)
        {
            // Remove old listeners from other stations
            interactButton.onClick.RemoveAllListeners();

            // Add CupStation's listener
            interactButton.onClick.AddListener(Interact);

            // Show button
            interactButton.gameObject.SetActive(true);
            Debug.Log("CupStation: Interact button shown");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        // Hide button when leaving
        interactButton.gameObject.SetActive(false);
    }

    public void Interact()
    {
        lastToggleTime = Time.time;

        PlayerDrinkManager.Instance.SetCupSize(CupSize.Small);
        Debug.Log("Player selected a Small cup");

        // Hide button after use
        interactButton.gameObject.SetActive(false);
    }
}
