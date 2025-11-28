using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using NUnit.Framework;
using UnityEngine.Rendering.Universal;

public enum Weather { Clear, Rain, Snow, Clouds}

public class WeatherManager : MonoBehaviour
{
    private string apiKey = "b1b3276c883b7bd3b0225ff49e755532";
    private float latitude;
    private float longitude;
    [SerializeField] private string cityName;
    private bool isDataReady;
    private string url;
    public Weather currentWeather;
    public Material[] skybox;

    private void Start()
    {
        StartCoroutine(GetLocation());
    }

    private void Update()
    {

    }

    public void GetWeather()
    {
        if (latitude != 0 || longitude != 0)
        {
            url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";
        }
        else
        {
            url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric";
        }

        StartCoroutine(FetchWeatherData(url));
    }

    private IEnumerator FetchWeatherData(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Got the data!
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Weather Data: " + jsonResponse);

                // Parse the JSON
                ParseWeatherData(jsonResponse);
            }
            else
            {
                // Error handling
                Debug.LogError("Error getting weather: " + request.error);
            }
        }
    }

    private void ParseWeatherData(string json)
    {
        WeatherApiResponse response = JsonUtility.FromJson<WeatherApiResponse>(json);

        float currentTemp = response.main.temp;
        string weatherCondition = response.weather[0].main;

        Debug.Log($"The weather in {response.name} is {weatherCondition} with a temp of {currentTemp}°C");

        ApplyWeatherToGame(weatherCondition);
    }

    private void ApplyWeatherToGame(string weatherCondition)
    {
        switch(weatherCondition)
        {
            case "Clear":
                Debug.Log("It's Sunny");
                currentWeather = Weather.Clear;
                RenderSettings.skybox = skybox[0];
                DynamicGI.UpdateEnvironment();
                break;
            case "Rain":
                Debug.Log("It's Rainy");
                currentWeather = Weather.Rain;
                RenderSettings.skybox = skybox[2];
                DynamicGI.UpdateEnvironment();
                break;
            case "Snow":
                Debug.Log("It's Snowy");
                currentWeather = Weather.Snow;
                RenderSettings.skybox = skybox[1];
                DynamicGI.UpdateEnvironment();
                break;
            case "Clouds":
                Debug.Log("It's Cloudy");
                currentWeather = Weather.Clouds;
                RenderSettings.skybox = skybox[1];
                DynamicGI.UpdateEnvironment();
                break;
            case "Thunderstorm":
                Debug.Log("It's Cloudy");
                currentWeather = Weather.Clouds;
                RenderSettings.skybox = skybox[2];
                DynamicGI.UpdateEnvironment();
                break;
            case "Drizzle":
                Debug.Log("It's Cloudy");
                currentWeather = Weather.Clouds;
                RenderSettings.skybox = skybox[1];
                DynamicGI.UpdateEnvironment();
                break;
        }
    }

    private IEnumerator GetLocation()
    {
        isDataReady = false;

        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services are not enabled by the user.");
            GetWeather();
            yield break; // Stop the coroutine
        }

        // Start the location service
        Debug.Log("Starting location service...");
        Input.location.Start();

        // Wait for the service to initialize
        int maxWaitTime = 20; // 20-second timeout
        while (Input.location.status == LocationServiceStatus.Initializing && maxWaitTime > 0)
        {
            yield return new WaitForSeconds(1); // Wait for 1 second
            maxWaitTime--;
        }

        // Check for errors
        if (maxWaitTime <= 0)
        {
            Debug.LogWarning("Location service timed out.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Location service failed to start.");
            yield break;
        }

        // On Success Get the data
        Debug.Log("Location service running.");
        LocationInfo locationData = Input.location.lastData;

        // Store the coordinates
        latitude = locationData.latitude;
        longitude = locationData.longitude;
        isDataReady = true;

        Debug.Log($"Location Acquired: {latitude}, {longitude}");

        GetWeather();

        // Stop the service
        Input.location.Stop();
        Debug.Log("Location service stopped.");
    }
}