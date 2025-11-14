using UnityEngine;
using System;

public enum Season { Spring, Summer, Autumn, Winter}

public class TimeManager : MonoBehaviour
{
    public Season currentSeason;
    public int currentDay;
    public int currentMonth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckCurrentDate();
    }

    public void CheckCurrentDate()
    {
        DateTime today = DateTime.Now;

        currentDay = today.Day;
        currentMonth = today.Month;

        switch (currentMonth)
        {
            case 1:
                //January
                currentSeason = Season.Winter;
                break;
            case 2:
                //February
                currentSeason = Season.Winter;
                break;
            case 3:
                //March
                currentSeason = Season.Winter;
                if (currentDay == 20)
                {
                    currentSeason = Season.Spring;
                }
                break;
            case 4:
                //April
                currentSeason = Season.Spring;
                break;
            case 5:
                //May
                currentSeason = Season.Spring;
                break;
            case 6:
                //June
                currentSeason = Season.Spring;
                if (currentDay == 21)
                {
                    currentSeason = Season.Summer;
                }
                break;
            case 7:
                //July
                currentSeason = Season.Summer;
                break;
            case 8:
                //August
                currentSeason = Season.Summer;
                break;
            case 9:
                //September
                currentSeason = Season.Summer;
                if (currentDay == 22)
                {
                    currentSeason = Season.Autumn;
                }
                break;
            case 10:
                //October
                currentSeason = Season.Autumn;
                break;
            case 11:
                //November
                currentSeason = Season.Autumn;
                break;
            case 12:
                //December
                currentSeason = Season.Autumn;
                if (currentDay == 20)
                {
                    currentSeason = Season.Winter;
                }
                break;
        }

        switch(currentDay, currentMonth)
        {
            case (1, 1):
            // New Year's Day
            break;
            case (14, 1):
            // Valentine's Day
            break;
            case (17, 3):
            // St. Patrick's Day
            break;
            case (1, 4):
            // April Fool
            break;
            case (31, 10):
            // Halloween
            break;
            case (1, 12):
            // Start Advent Calendar Event
            break;
            case (24, 12):
            // Christma's Eve
            break;
            case (25, 12):
            // Christma's Day
            break;
            case (31, 12):
            //New Year's Eve
            break;
        }
    }
}
