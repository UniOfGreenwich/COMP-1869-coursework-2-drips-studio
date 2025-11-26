using UnityEngine;
using UnityEngine.UI;
using static DrinkIngredientsEnum;

public class MilkStation : MonoBehaviour
{
    private bool playerInside = false;
    private float lastToggleTime = 0f;
    private RandomSoundEffectTrigger trigger;

    [SerializeField] private float toggleCooldown = 1f; // seconds between toggles
    public Button interactButton;

    private void Awake()
    {
        if (interactButton == null)
            Debug.LogError("MilkStation: interactButton reference is missing!");
    }

    private void Start()
    {
        // Set up button once
        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(Interact);

        // Hide button at start
        interactButton.gameObject.SetActive(false);

        trigger = GetComponent<RandomSoundEffectTrigger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        // Show button only if cooldown allows
        if (Time.time - lastToggleTime >= toggleCooldown)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(Interact);

            interactButton.gameObject.SetActive(true);
            Debug.Log("MilkStation: Interact button shown");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
        interactButton.gameObject.SetActive(false);
    }

    public void Interact()
    {
        lastToggleTime = Time.time;

        PlayerDrinkManager.Instance.ToggleAdditive(Additive.Milk);
        Debug.Log("Player added milk to the drink!");

        // Hide button after use
        interactButton.gameObject.SetActive(false);

        trigger.Play();
    }
}
