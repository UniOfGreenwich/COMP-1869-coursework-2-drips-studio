using System.Collections.Specialized;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;
using static DrinkIngredientsEnum;

public class CheckingStation : MonoBehaviour
{
    private DrinkComparer drinkComparer;
    private TicketInstance ticketInstance;
    private TicketManager ticketManager;
    private PlayerDrinkManager playerDrinkManager;
    private InteractSquishAnimation tipJar;
    private SlideMenuValues slideMenuValues;
    private InGameUIManager inGameUIManager;
    private Player player;
    private bool doOnce = true;

    [Header("Cooldown Settings")]
    [SerializeField] private float activationCooldown = 1.0f; // seconds
    private float lastActivationTime = 0f;

    [Header("References")]
    public RandomSoundEffectTrigger correctTrigger;
    public RandomSoundEffectTrigger incorrectTrigger;
    private InteractSquishAnimation squish;

    private bool playerInside = false;
    public Button interactButton;

    private void Awake()
    {
        // Automatically find references in the scene
        drinkComparer = FindObjectOfType<DrinkComparer>();
        playerDrinkManager = FindAnyObjectByType<PlayerDrinkManager>();

        if (drinkComparer == null)
            Debug.LogError("No DrinkComparer found in the scene!");

        ticketManager = FindAnyObjectByType<TicketManager>();
        tipJar = GameObject.Find("Tip Jar")?.GetComponent<InteractSquishAnimation>();

        if (interactButton == null)
            Debug.LogError("CheckingStation: interactButton reference missing!");

        slideMenuValues = FindAnyObjectByType<SlideMenuValues>();
        inGameUIManager = FindAnyObjectByType<InGameUIManager>();
        player = FindAnyObjectByType<Player>();
    }

    private void Start()
    {
        // Set up button once
        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(Interact);

        // Hide button initially
        interactButton.gameObject.SetActive(false);

        squish = GetComponent<InteractSquishAnimation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        ticketInstance = FindObjectOfType<TicketInstance>();
        if (ticketInstance == null)
            Debug.LogError("No TicketInstance found in the scene!");

        // Only show button if cooldown is ready
        if (Time.time - lastActivationTime >= activationCooldown)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(Interact);
            interactButton.gameObject.SetActive(true);
            Debug.Log("CheckingStation: Interact button shown");
        }
        doOnce = true;
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
        lastActivationTime = Time.time;

        if (PlayerDrinkManager.Instance == null)
        {
            Debug.LogWarning("PlayerDrinkManager not found!");
            return;
        }

        if (ticketInstance == null || ticketInstance.customerOrder == null)
        {
            Debug.LogWarning("TicketInstance or customer order missing!");
            return;
        }

        DrinkPresets playerDrink = ScriptableObject.CreateInstance<DrinkPresets>();
        playerDrink.cupSize = PlayerDrinkManager.Instance.cupSize;
        playerDrink.espresso = PlayerDrinkManager.Instance.espresso;
        playerDrink.additives = new System.Collections.Generic.List<Additive>(PlayerDrinkManager.Instance.additives);

        bool drinksMatch = drinkComparer.CompareDrinks(playerDrink, ticketInstance.customerOrder);

        if (drinksMatch)
        {
            Debug.Log("Correct drink served!");
            ticketManager.currentTickets = 0;
            playerDrinkManager.ResetDrink();

            tipJar.squish = true;
            ParticleSystem moneySpill = tipJar.transform.Find("Money Spill")?.GetComponent<ParticleSystem>();
            moneySpill?.Play();
            correctTrigger.Play();
            squish.squish = true;
            slideMenuValues.customersServed++;
            slideMenuValues.customersServedCorrectly++;
            inGameUIManager.UpdateReputation();
            ticketManager.RemoveTicket(ticketInstance.gameObject);
            if (doOnce) inGameUIManager.AddMoney();

        }
        else
        {
            Debug.Log("Wrong drink served!");
            incorrectTrigger.Play();
            squish.squish = true;
            ticketManager.currentTickets = 0;
            playerDrinkManager.ResetDrink();
            slideMenuValues.customersServed++;
            inGameUIManager.UpdateReputation();
            ticketManager.RemoveTicket(ticketInstance.gameObject);
        }

        doOnce = false;
        Destroy(playerDrink);

        // Hide button after interaction
        interactButton.gameObject.SetActive(false);
    }
}
