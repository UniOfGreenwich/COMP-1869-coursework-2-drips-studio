using UnityEngine;
using static DrinkIngredientsEnum;

public class CoffeeStation : MonoBehaviour
{
    private bool playerInside = false;
    private float lastToggleTime = 0f;
    [SerializeField] private float toggleCooldown = 1f; // seconds between toggles

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mark player inside
            playerInside = true;

            // Only allow toggle if cooldown has passed
            if (Time.time - lastToggleTime >= toggleCooldown)
            {
                lastToggleTime = Time.time;
                PlayerDrinkManager.Instance.SetEspresso(EspressoAmount.One);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
