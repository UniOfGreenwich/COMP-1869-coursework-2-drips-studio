using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField] private Sprite profilePicture;
    [SerializeField] private DateTime quitTimePlayer;
    [SerializeField] private string quitTimeText;
    [SerializeField] private int quitTimeHour;
    [SerializeField] private int quitTimeMinute;
    [SerializeField] private int money;
    [SerializeField] GameObject[] meshRenderer = new GameObject[3];

    private void Awake()
    {
        meshRenderer[PlayerProfile.AvatarIndex].SetActive(true);
    }

    public void SaveData(ref PlayerData data)
    {
        //data.profilePicture = profilePicture;
        data.quitTime = quitTimeText;
        data.quitHour = quitTimeHour;
        data.quitMinute = quitTimeMinute;
        data.money = money;
    }

    public void LoadData(PlayerData data)
    {
        //profilePicture = data.profilePicture;
        quitTimeText = data.quitTime;
        quitTimeHour = data.quitHour;
        quitTimeMinute = data.quitMinute;
        money = data.money;
    }

    //public Sprite GetProfilePicture()
    //{
    //    return profilePicture;
    //}

    //public void SetProfilePicture(Sprite selectedPicture)
    //{
    //    profilePicture = selectedPicture;
    //}

    public string GetQuitTime()
    {
        return quitTimeText;
    }

    public void SetQuitTime(DateTime currentTime)
    {
        quitTimePlayer = currentTime;
        quitTimeHour = currentTime.Hour;
        quitTimeMinute = currentTime.Minute;
        quitTimeText = ConvertTimeToText(quitTimePlayer);
    }

    public int GetQuitTimeHour()
    {
        return quitTimeHour;
    }

    public int GetQuitTimeMinute()
    {
        return quitTimeMinute;
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
        string currentTime = String.Format("{0:00}:{1:00}",  currentHour, currentMinute);

        return currentTime;
    }
}
