using UnityEngine;

public class TrashCanStation : MonoBehaviour
{
    [Header("Trash Can Capacity")]
    [SerializeField] private int binLevel;
    [SerializeField] private int binMaxCapacity;

    private void Start()
    {
        binLevel = 0;
    }

    private void Update()
    {
        if (binLevel > binMaxCapacity) binLevel = binMaxCapacity;
    }

    public void IncreaseBinLevel()
    {
        binLevel++;
    }

    public void ResetBinLevel()
    {
        binLevel = 0;
    }

    void OnTriggerEnter()
    {
        ResetBinLevel();
    }
}
