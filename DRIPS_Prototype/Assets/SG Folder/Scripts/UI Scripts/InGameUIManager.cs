using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class InGameUIManager : MonoBehaviour
{
    [Header("Top Bar")]
    public TMP_Text dayText;
    public TMP_Text timeText;
    public TMP_Text moneyText;

    [Header("References")]
    public GameObject canvas;
    public GameObject interactButton;
    public GameObject player;


    [Header("Panels")]
    public GameObject sideMenuPanel;
    public GameObject settingsPanel;
    public GameObject rewardsPanel;
    public  GameObject dailyRewardsPanel;
    public  GameObject shopPanel;

    private bool sideMenuOpen = false;
    private bool settingsOpen = false;
    private bool rewardsOpen = false;
    private bool dailyRewardsOpen = false;
    private bool shopOpen = false;

    private void Start()
    {
        // make sure main canvas is enabled on start
        canvas.SetActive(true);

        // make sure panels start closed
        sideMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        rewardsPanel.SetActive(false);
        dailyRewardsPanel.SetActive(false);
        shopPanel.SetActive(false);
    }

    public void ToggleSideMenu()
    {
        sideMenuOpen = !sideMenuOpen;
        sideMenuPanel.SetActive(sideMenuOpen);
    }

    public void ToggleSettings()
    {
        settingsOpen = !settingsOpen;
        settingsPanel.SetActive(settingsOpen);

        // Optional?? pause game while settings open??
        Time.timeScale = settingsOpen ? 0f : 1f;
    }

    public void ToggleRewardsMenu()
    {
        rewardsOpen = !rewardsOpen;
        ToggleDailyRewardsPanel();
        
        rewardsPanel.SetActive(rewardsOpen);
    }

    public void ToggleDailyRewardsPanel()
    {
        shopOpen = false;
        shopPanel.SetActive(false);
        dailyRewardsOpen = true;
        dailyRewardsPanel.SetActive(true);
    }

    public void ToggleShopPanel()
    {
        shopOpen = true;
        shopPanel.SetActive(true);
        dailyRewardsOpen = false;
        dailyRewardsPanel.SetActive(false);
    }


    public void UpdateHUD(int day, float timeOfDay, float money)
    {
        dayText.text = $"DAY {day}";
        timeText.text = $"TIME {timeOfDay:00:00}";
        moneyText.text = $"MONEY ${money:0.00}";
    }

    public void InteractButton()
    {
        // create a sphere radius of 5 units
        // store vector 3 positions in a list from objects named "Position"
        // find the parent object of the closest one
        // check if the parent object has any of these component scripts: "CoffeeStation", "CupStation", "MilkStation", "ServingStation", "TrashCanStation"
        // if yes, create a reference to that component. if not, Debug "No Station Script Found"

    }
}
