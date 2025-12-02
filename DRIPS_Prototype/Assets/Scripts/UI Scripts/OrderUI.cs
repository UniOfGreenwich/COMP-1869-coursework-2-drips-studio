using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUI : MonoBehaviour
{
    public OrderManager orderManager;
    public TMP_Text orderText;

    // Ishan Added to integrate for Prototype
    [SerializeField] private GameObject generateTakeOrderButton;
    [SerializeField] private GameObject makeOrderButton;
    [SerializeField] private GameObject serveOrderButton;

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

    // Ishan added this to integrate for Hi-Fi prototype
    public void OnGenerateAndTakeOrder()
    {
        OnGenerateOrder();
        OnTakeOrder();
    }

    public void TurnOffButtons()
    {
        serveOrderButton.gameObject.SetActive(false);
        makeOrderButton.gameObject.SetActive(false);
        generateTakeOrderButton.gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if (orderManager.currentOrder != null)
            orderText.text = "Current Order: " + orderManager.currentOrder.drinkName;
        else
            orderText.text = "No active order.";
    }
}
