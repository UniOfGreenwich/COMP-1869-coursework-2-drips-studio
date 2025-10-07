using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    // List of drinks
    private List<string> drinkMenu = new List<string>()
    {
        "Latte",
        "Cappuccino",
        "Espresso",
        "Mocha"
    };

    public Order currentOrder { get; private set; }

    // Generate Random Order
    public void GenerateOrder()
    {
        int randomIndex = Random.Range(0, drinkMenu.Count);
        string drink = drinkMenu[randomIndex];
        currentOrder = new Order(drink);
        Debug.Log("Customer ordered: " + drink);
    }

    public void TakeOrder()
    {
        if (currentOrder == null)
        {
            Debug.Log("No order to take!");
            return;
        }

        Debug.Log("Player took the order: " + currentOrder.drinkName);
    }

    public void MakeOrder()
    {
        if (currentOrder == null)
        {
            Debug.Log("No order to make!");
            return;
        }

        Debug.Log("Player is making: " + currentOrder.drinkName);
    }

    public void CompleteOrder()
    {
        if (currentOrder == null)
        {
            Debug.Log("No order to complete!");
            return;
        }

        Debug.Log("Order completed! Gave customer: " + currentOrder.drinkName);
        currentOrder = null; 
    }
}
