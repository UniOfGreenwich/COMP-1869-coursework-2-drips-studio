using System.Collections.Generic;
using UnityEngine;
using static DrinkIngredientsEnum;

public class PlayerDrinkManager : MonoBehaviour
{
    public static PlayerDrinkManager Instance;

    [Header("Current Player Drink")]
    public CupSize cupSize = CupSize.Small;
    public EspressoAmount espresso = EspressoAmount.Zero;
    public List<Additive> additives = new List<Additive>();

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ResetDrink();
    }

    public void ResetDrink()
    {
        cupSize = CupSize.Small;
        espresso = EspressoAmount.Zero;
        additives.Clear();
        Debug.Log("Player drink reset to empty.");
    }

    public void SetCupSize(CupSize newSize)
    {
        cupSize = newSize;
        Debug.Log($"Set cup size: {cupSize}");
    }

    public void SetEspresso(EspressoAmount amount)
    {
        espresso = amount;
        Debug.Log($"Set espresso amount: {espresso}");
    }

    public void ToggleAdditive(Additive additive)
    {
        if (additives.Contains(additive))
        {
            additives.Remove(additive);
            Debug.Log($"Removed additive: {additive}");
        }
        else
        {
            additives.Add(additive);
            Debug.Log($"Added additive: {additive}");
        }
    }

    public string GetDrinkSummary()   // For debugging if needed
    {
        string additiveList = additives.Count > 0 ? string.Join(", ", additives) : "None";
        return $"Drink: {cupSize} cup, {espresso} espresso, Additives: {additiveList}";
    }
}
