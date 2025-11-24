using UnityEngine;
using UnityEngine.UI;

public class TrashCanStation : MonoBehaviour
{
    [Header("Trash Can Capacity")]
    [SerializeField] private int binLevel;
    [SerializeField] private int binMaxCapacity;

    public Button interactButton; // Assign in inspector

    private void Start()
    {
        binLevel = 0;

        // Setup button
        if (interactButton != null)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(ResetBinLevel);
            interactButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("TrashCanStation: interactButton reference missing!");
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Show interact button
        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Hide button when leaving
        interactButton?.gameObject.SetActive(false);
    }
}
