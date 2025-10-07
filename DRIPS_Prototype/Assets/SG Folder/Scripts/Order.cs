using System;

[System.Serializable]
public class Order
{
    public string drinkName;

    public Order(string drinkName)
    {
        this.drinkName = drinkName;
    }
}
