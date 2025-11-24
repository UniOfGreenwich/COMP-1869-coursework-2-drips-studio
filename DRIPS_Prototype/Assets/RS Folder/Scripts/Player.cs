using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerData playerData;
    [SerializeField] private Sprite profilePicture;
    [SerializeField] private DateTime quitTime;
    [SerializeField] private string quitTimeText;
    [SerializeField] private int money;

    private void Start()
    {
        playerData = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerData>();
    }

    public void SaveData(ref PlayerData data)
    {
        data.profilePicture = profilePicture;
        data.quitTime = quitTimeText;
        data.money = money;
    }

    public void LoadData(PlayerData data)
    {
        profilePicture = data.profilePicture;
        quitTimeText = data.quitTime;
        money = data.money;
    }

    public Sprite GetProfilePicture()
    {
        return profilePicture;
    }

    public void SetProfilePicture(Sprite selectedPicture)
    {
        profilePicture = selectedPicture;
    }

    public string GetQuitTime()
    {
        return ConvertTimeToText(quitTime);
    }

    public void SetQuitTime(DateTime currentTime)
    {
        quitTime = currentTime;
        quitTimeText = ConvertTimeToText(quitTime);
    }

    public int GetPlayerMoney()
    {
        return money;
    }

    public void SetPlayerMoney(int amount)
    {
        money += amount;
    }

    public string ConvertTimeToText(DateTime time)
    {
        int currentHour = time.Hour;
        int currentMinute = time.Minute;
        string currentTime = currentHour + ":" + currentMinute;

        return currentTime;
    }
}
