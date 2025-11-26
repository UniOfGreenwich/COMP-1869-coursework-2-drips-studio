using UnityEngine;
using UnityEngine.UI;

public class TrashCanStation : MonoBehaviour
{
    [Header("Trash Can Capacity")]
    [SerializeField] private int binLevel;
    [SerializeField] private int binMaxCapacity;

    [Header("Interact Button")]
    public Button interactButton; // Assign in Inspector

    [Header("Cooldown Settings")]
    [SerializeField] private float toggleCooldown = 0.5f; // optional, prevents accidental double clicks
    private float lastToggleTime = 0f;

    private bool playerInside = false;

    private void Awake()
    {
        if (interactButton == null)
            Debug.LogError("TrashCanStation: interactButton reference missing!");
    }

    private void Start()
    {
        binLevel = 0;

        // Hide button at start
        if (interactButton != null)
            interactButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (binLevel > binMaxCapacity)
            binLevel = binMaxCapacity;
    }

    public void IncreaseBinLevel()
    {
        binLevel++;
    }

    public void ResetBinLevel()
    {
        binLevel = 0;
        Debug.Log("Trash can emptied!");

        // Hide button after use
        interactButton?.gameObject.SetActive(false);
    }

    public void Interact()
    {
        lastToggleTime = Time.time;
        ResetBinLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        // Show button only if cooldown allows
        if (Time.time - lastToggleTime >= toggleCooldown)
        {
            if (interactButton != null)
            {
                interactButton.onClick.RemoveAllListeners();
                interactButton.onClick.AddListener(Interact);
                interactButton.gameObject.SetActive(true);
                Debug.Log("TrashCanStation: Interact button shown");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
        interactButton?.gameObject.SetActive(false);
    }
}
