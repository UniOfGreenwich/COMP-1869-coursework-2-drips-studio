using UnityEngine;

[System.Serializable]
public class WeatherApiResponse
{
    public WeatherInfo[] weather; // This is an array
    public MainWeather main;
    public string name; // City name
}
