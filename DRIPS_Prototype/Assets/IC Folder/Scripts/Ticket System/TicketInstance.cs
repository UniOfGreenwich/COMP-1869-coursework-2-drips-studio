using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TicketInstance : MonoBehaviour
{
    [Header("Preset Library")]
    public List<DrinkPresets> availableDrinks;

    [Header("Current Drink Order")]
    public DrinkPresets currentOrder;

    void Start()
    {
        GenerateOrder();
    }

    public void GenerateOrder()
    {
        if (availableDrinks.Count == 0)
        {
            Debug.LogWarning("No drink presets assigned!");
            return;
        }

        currentOrder = availableDrinks[Random.Range(0, availableDrinks.Count)];
        Debug.Log($"Generated order: {currentOrder.drinkName}");
    }
}
