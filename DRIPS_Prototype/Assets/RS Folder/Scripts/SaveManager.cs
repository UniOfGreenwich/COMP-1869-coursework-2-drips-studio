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

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }    

    private static void HandleSaveData()
    {
        GameManager.Instance.player.SaveData(ref saveData.playerData);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        GameManager.Instance.player.LoadData(saveData.playerData);
    }
}