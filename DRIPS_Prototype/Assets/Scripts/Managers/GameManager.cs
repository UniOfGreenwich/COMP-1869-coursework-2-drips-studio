using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //Creating Singleton
    #region
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

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
        spawner = GameObject.FindGameObjectWithTag("CustomerManager").GetComponent<CustomerSpawner>();
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneTransfer>();
        queue = GameObject.FindGameObjectWithTag("CustomerManager").GetComponent<QueueManager>();
        seatingManager = GameObject.FindGameObjectWithTag("CustomerManager").GetComponent<SeatingManager>();
        ticketManager = GameObject.FindGameObjectWithTag("TicketManager").GetComponent<TicketManager>();
    }

    private void Update()
    {
        UpdateUI();
        if (open)
        {
            HandleWorkingShift();
        }
    }

    //Save/Load functionality
    #region
    public Player player;

    

    private void Start()
    {
        SaveManager.Load();
    }

    public void SaveButton()
    {
        if (player != null)
        {
            player.SetQuitTime(timeManager.today);
        }
        SaveManager.Save();
    }

    public void LoadButton()
    {
        SaveManager.Load();
        load = true;
    }

    private void OnApplicationPause()
    {
        if(Time.time > 1f)
        {
            SaveButton();
        }
    }

    public void AddMoney(int amount)
    {
        if (player != null)
        {
            player.SetPlayerMoney(amount);
        }
    }
    #endregion

    //UI related
    #region
    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI money;
    public TextMeshProUGUI currentDate;
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

            if (currentDate != null)
            {
                currentDate.text = timeManager.CheckCurrentDate();
            }
        }

        if (money != null)
        {
            money.text = player.GetPlayerMoney().ToString();
        }
    }
    #endregion

    //Opening Cafe
    #region
    public CustomerSpawner spawner;
    public SceneTransfer sceneManager;
    public QueueManager queue;
    public SeatingManager seatingManager;
    public TicketManager ticketManager;
    public bool open;
    public float shiftTime = 300;

    public void OpenButton()
    {
        open = true;
        spawner.StartCoroutine(spawner.InitLoop());
    }

    private void HandleWorkingShift()
    {
        shiftTime -= Time.deltaTime;
        if (shiftTime <= 0f)
        {
            spawner.StopAllCoroutines();
            open = false;
            
            GameObject[] customerInScene = GameObject.FindGameObjectsWithTag("Customer");
            if (customerInScene.Length > 0)
            {
                foreach (GameObject customer in customerInScene)
                {
                    Destroy(customer);
                }
            }
            
            GameObject[] splatters = GameObject.FindGameObjectsWithTag("PoI Splatter");
            if (splatters.Length > 0)
            {
                foreach (GameObject splatter in splatters)
                {
                    Destroy(splatter);
                }
            }
            
            spawner.ResetSpawner();
            queue.ResetQueue();
            seatingManager.ResetSeats();
            ticketManager.ResetTickets();
            PlayerDrinkManager.Instance.ResetDrink();

            shiftTime = 300;

        }        
    }
    #endregion
}