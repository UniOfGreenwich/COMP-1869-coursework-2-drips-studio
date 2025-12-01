using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

public class InGameUIManager : MonoBehaviour
{
    [Header("Top Bar")]
    public TMP_Text dayText;
    public TMP_Text timeText;
    public TMP_Text moneyText;

    [Header("Slide Menu")]
    public TMP_Text customersServedText;
    public TMP_Text customersServedCorrectlyText;
    public TMP_Text reputationText;

    [Header("References")]
    public GameObject canvas;
    public GameObject interactButton;
    public GameObject player;
    public SlideMenuValues slideMenuValues;


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

        // Find References
        slideMenuValues = FindAnyObjectByType<SlideMenuValues>();
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

        if (rewardsOpen)
        {
            rewardsOpen = !rewardsOpen;
            rewardsPanel.SetActive(rewardsOpen);
        }
        

        // Optional?? pause game while settings open??
        Time.timeScale = settingsOpen ? 0f : 1f;
    }

    public void ToggleRewardsMenu()
    {
        rewardsOpen = !rewardsOpen;
        ToggleDailyRewardsPanel();
        
        rewardsPanel.SetActive(rewardsOpen);

        if (settingsOpen)
        {
            settingsOpen = !settingsOpen;
            settingsPanel.SetActive(!rewardsPanel);
        }
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

    public void IncrementCustomersServed()
    {
        customersServedText.text = "Customers Served: " + slideMenuValues.customersServed;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.open)
        {
            customersServedText.text = "Customers Served: " + slideMenuValues.customersServed;
            customersServedCorrectlyText.text = "Customers Served Correctly: " + slideMenuValues.customersServedCorrectly;
        }
    }
    public void UpdateReputation()
    {
        if (slideMenuValues.customersServed == 0)
        {
            reputationText.text = "Reputation: N/A";
            return;
        }

        float rep = (float)slideMenuValues.customersServedCorrectly / slideMenuValues.customersServed * 100f;
        reputationText.text = "Reputation: " + Mathf.RoundToInt(rep) + "%";
    }
}