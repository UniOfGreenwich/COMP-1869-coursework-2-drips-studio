using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TicketInstance : MonoBehaviour
{
    [Header("Preset Library")]
    public List<DrinkPresets> availableDrinks;

    [Header("Current Drink Order")]
    public DrinkPresets customerOrder;

    void Start()
    {
        LoadAvailableDrinks();
        GenerateOrder();
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
    }

    void LoadAvailableDrinks()
    {
        DrinkPresets[] found = Resources.LoadAll<DrinkPresets>("");
        availableDrinks = new List<DrinkPresets>(found);
        Debug.Log($"Loaded {availableDrinks.Count} drink presets.");
    }
}
