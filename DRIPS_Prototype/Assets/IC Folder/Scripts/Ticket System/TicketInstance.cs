using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TicketInstance : MonoBehaviour
{
    [Header("Preset Library")]
    public List<DrinkPresets> availableDrinks;

    [Header("Current Drink Order")]
    public DrinkPresets customerOrder;

    [Header("Patience Timer")]
    public Slider patienceSlider;
    public Image fillImage;
    public Color targetColor = new Color(0.713f, 0.361f, 0.373f);
    private Color startColor;

    private float timerDuration = 15f;     // 15 seconds
    private float timerRemaining;
    public TicketManager tm;

    void Start()
    {
        LoadAvailableDrinks();
        GenerateOrder();

        tm = FindAnyObjectByType<TicketManager>();

        // Initialize timer
        timerRemaining = timerDuration;
        patienceSlider.maxValue = 1f;
        patienceSlider.value = 1f;
        startColor = fillImage.color;
    }

    void Update()
    {
        if (timerRemaining > 0)
        {
            timerRemaining -= Time.deltaTime;
            patienceSlider.value = timerRemaining / timerDuration;

            // Update slider fill color based on remaining patience
            float t = 1f - patienceSlider.value; // 0 = full, 1 = empty
            fillImage.color = Color.Lerp(startColor, targetColor, t);
        }
        else
        {
            if (timerRemaining < 0)
            {
                tm.RemoveTicket(gameObject);
            }
        }
    }

    public void GenerateOrder()
    {
        if (availableDrinks.Count == 0)
        {
            Debug.LogWarning("No drink presets assigned!");
            return;
        }

        customerOrder = availableDrinks[Random.Range(0, availableDrinks.Count)];
        Debug.Log("Generated customer order");

        // Reset patience timer for new order
        timerRemaining = timerDuration;
        patienceSlider.value = 1f;
    }

    void LoadAvailableDrinks()
    {
        DrinkPresets[] found = Resources.LoadAll<DrinkPresets>("");
        availableDrinks = new List<DrinkPresets>(found);
        Debug.Log($"Loaded {availableDrinks.Count} drink presets.");
    }
}
