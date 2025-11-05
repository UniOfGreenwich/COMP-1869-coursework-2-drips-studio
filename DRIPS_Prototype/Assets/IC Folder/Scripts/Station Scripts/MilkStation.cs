using UnityEngine;
using static DrinkIngredientsEnum;

public class MilkStation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only respond to the player
        if (other.CompareTag("Player"))
        {
            PlayerDrinkManager.Instance.ToggleAdditive(Additive.Milk);
            Debug.Log("Player added milk to the drink!");
        }
    }
}
