using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Player player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        timeManager = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<TimeManager>();
    }

    public void SaveButton()
    {
        SaveManager.Save();
    }

    public void LoadButton()
    {
        SaveManager.Load();
        load = true;
    }

    //private void OnApplicationPause()
    //{
    //    if (player != null)
    //    {
    //        player.SetQuitTime(timeManager.today);
    //    }
    //    SaveButton();
    //}
    private void OnApplicationQuit()
    {
        //if (player != null)
        //{
        //    player.SetQuitTime(timeManager.today);
        //}
        SaveButton();
    }

    public void AddMoney(int amount)
    {
        if (player != null)
        {
            player.SetPlayerMoney(amount);
        }
    }

    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI time;
    public TextMeshProUGUI money;
    public TimeManager timeManager;
    public bool load;

    void UpdateUI()
    {
        if (timeManager != null)
        {
            if (currentTime != null)
            {
                currentTime.text = timeManager.currentTime;
            }

            if (time != null && load)
            {
                time.text = player.GetQuitTime();
                load = false;
            }
        }

        if (money != null)
        {
            money.text = player.GetPlayerMoney().ToString();
        }
    }

    private void Update()
    {
        UpdateUI();
    }

}
