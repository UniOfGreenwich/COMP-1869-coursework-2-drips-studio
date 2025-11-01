using UnityEngine;

public class DrinkIngredientsEnum
{
    // Types of Cup Sizes
    public enum CupSize
    {
        Small,
        Large
    }

    // Number of espresso shots
    public enum EspressoAmount
    {
        Zero,
        One,
        Two
    }

    // Possible additives or ingredients
    public enum Additive
    {
        Water,
        Milk,
        ChocolatePowder,
        ChocolateSyrup,
        HazelnutSyrup
    }
}
