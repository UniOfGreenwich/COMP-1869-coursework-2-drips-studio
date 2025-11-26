using UnityEngine;
using System.IO;

public class SaveManager
{
    private static SaveData saveData = new SaveData();

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/meow" + ".uwu";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        string saveFilePath = SaveFileName();
        string json = JsonUtility.ToJson(saveData, true);
        try
        {
            // This is the line that is failing on Android
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Save successful.");
        }
        catch (System.Exception e)
        {
            //THIS IS THE KEY: Log the actual error to the console/Logcat
            Debug.LogError("SAVE FAILED! Actual error on Android: " + e.GetType().Name + " - " + e.Message);
        }
    }    

    private static void HandleSaveData()
    {
        GameManager.Instance.player.SaveData(ref saveData.playerData);
    }

    public static void Load()
    {
        string saveFilePath = SaveFileName();

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found or write failed. Loading defaults.");
            // If file doesn't exist, we exit and keep the default saveData values.
            return;
        }

        try
        {
            string saveContent = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(saveContent);
            HandleLoadData();
            Debug.Log("Load successful.");
        }
        catch (System.Exception e)
        {
            // Handle potential corruption or malformed JSON
            Debug.LogError("LOAD FAILED! Data might be corrupted: " + e.Message);
        }
    }

    private static void HandleLoadData()
    {
        GameManager.Instance.player.LoadData(saveData.playerData);
    }
}