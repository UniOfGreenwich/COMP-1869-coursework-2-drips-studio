using UnityEngine;
using UnityEngine.UI;
using static DrinkIngredientsEnum;

public class CoffeeStation : MonoBehaviour
{
    private bool playerInside = false;
    private float lastToggleTime = 0f;
    private RandomSoundEffectTrigger trigger;

    [SerializeField] private float toggleCooldown = 1f;
    public Button interactButton;

    private ParticleSystem steam;

    private void Awake()
    {
        if (interactButton == null)
            Debug.LogError("CoffeeStation: interactButton reference missing!");
    }

    private void Start()
    {
        // Find steam
        steam = GetComponentInChildren<ParticleSystem>();

        // Hide button initially
        interactButton.gameObject.SetActive(false);

        trigger = GetComponent<RandomSoundEffectTrigger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        // Only allow interaction if cooldown is ready
        if (Time.time - lastToggleTime >= toggleCooldown)
        {
            // Remove old listeners from other stations
            interactButton.onClick.RemoveAllListeners();

            // Add THIS station's listener
            interactButton.onClick.AddListener(Interact);

            // Show button
            interactButton.gameObject.SetActive(true);
            Debug.Log("CoffeeStation: Interact button shown");
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

        PlayerDrinkManager.Instance.SetEspresso(EspressoAmount.One);
        Debug.Log("Player added espresso!");

        steam?.Play();

        // Hide button after use
        interactButton.gameObject.SetActive(false);

        trigger.Play();
    }
}
