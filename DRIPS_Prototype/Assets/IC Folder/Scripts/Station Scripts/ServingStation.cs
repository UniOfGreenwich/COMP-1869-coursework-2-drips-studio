using UnityEngine;
using static DrinkIngredientsEnum;

public class CheckingStation : MonoBehaviour
{
    private DrinkComparer drinkComparer;
    private TicketInstance ticketInstance;
    private TicketManager ticketManager;
    private PlayerDrinkManager playerDrinkManager;
    private InteractSquishAnimation tipJar;
    

    [Header("Cooldown Settings")]
    [SerializeField] private float activationCooldown = 1.0f; // seconds
    private float lastActivationTime = 0f;

    private bool playerInside = false;

    private void Awake()
    {
        // Automatically find references in the scene
        drinkComparer = FindObjectOfType<DrinkComparer>();

        playerDrinkManager = FindAnyObjectByType<PlayerDrinkManager>();

        if (drinkComparer == null)
            Debug.LogError("No DrinkComparer found in the scene!");

        ticketManager = FindAnyObjectByType<TicketManager>();

        tipJar = GameObject.Find("Tip Jar")?.GetComponent<InteractSquishAnimation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;


        ticketInstance = FindObjectOfType<TicketInstance>();
        if (ticketInstance == null)
            Debug.LogError("No TicketInstance found in the scene!");

        // Check if enough time passed since last activation
        if (Time.time - lastActivationTime >= activationCooldown)
        {
            lastActivationTime = Time.time;
            RunDrinkCheck();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private void RunDrinkCheck()
    {
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
            moneySpill.Play();
        }
        else
        {
            Debug.Log("Wrong drink served!");
            playerDrinkManager.ResetDrink();
        }

        Destroy(playerDrink);
    }
}
