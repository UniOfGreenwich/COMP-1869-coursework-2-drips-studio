using System.Collections.Generic;
using UnityEngine;
using static DrinkIngredientsEnum;


// You can make drink presets by right clicking the project window and hovering over "Coffee"
[CreateAssetMenu(fileName = "DrinkPresets", menuName = "Coffee/DrinkPresets")]
public class DrinkPresets : ScriptableObject
{
    // There are some options you must choose, others that are optional
    public CupSize cupSize;
    public EspressoAmount espresso;
    public List<Additive> additives;
}
