using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUI : MonoBehaviour
{
    public OrderManager orderManager;
    public TMP_Text orderText;

    public void OnGenerateOrder()
    {
        orderManager.GenerateOrder();
        UpdateUI();
    }

    public void OnTakeOrder()
    {
        orderManager.TakeOrder();
    }

    public void OnMakeOrder()
    {
        orderManager.MakeOrder();
    }

    public void OnCompleteOrder()
    {
        orderManager.CompleteOrder();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (orderManager.currentOrder != null)
            orderText.text = "Current Order: " + orderManager.currentOrder.drinkName;
        else
            orderText.text = "No active order.";
    }
}
