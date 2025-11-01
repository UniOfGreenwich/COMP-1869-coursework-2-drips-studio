using System.Collections.Generic;
using UnityEngine;
using static DrinkIngredientsEnum;

public class DrinkComparer : MonoBehaviour
{
    public bool CompareDrinks(DrinkPresets playerDrink, DrinkPresets ticketDrink)
    {
        if (playerDrink == null || ticketDrink == null)
        {
            Debug.LogWarning("One or both drinks are null!");
            return false;
        }

        // Compare cup size
        if (playerDrink.cupSize != ticketDrink.cupSize)
        {
            Debug.Log($"Cup size mismatch: {playerDrink.cupSize} vs {ticketDrink.cupSize}");
            return false;
        }

        // Compare espresso shots
        if (playerDrink.espresso != ticketDrink.espresso)
        {
            Debug.Log($"Espresso amount mismatch: {playerDrink.espresso} vs {ticketDrink.espresso}");
            return false;
        }

        // Compare additives
        if (!AreAdditivesEqual(playerDrink.additives, ticketDrink.additives))
        {
            Debug.Log("Additives mismatch!");
            return false;
        }

        Debug.Log("Drinks match perfectly!");
        return true;
    }


    // Helper function for specifically checking if Additives match
    private bool AreAdditivesEqual(List<Additive> a, List<Additive> b)
    {
        if (a == null || b == null)
            return a == b;   // If customer drinks don't have additives

        if (a.Count != b.Count)
            return false;   // if for whatever reason the additive lists aren't the same size, return false and cry because something must be broken

        foreach (var additive in a)
        {
            if (!b.Contains(additive))
                return false;
        }

        return true;
    }   
}
