using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransfer : MonoBehaviour
{
    public void MapToGame()
    {
        SceneManager.LoadScene("Prototype Scene");
    }

    public void GameToMenu()
    {
        SceneManager.LoadScene("Start_Scene");
    }

    public void MenuToProfile()
    {
        SceneManager.LoadScene("Create_Profile_Scene");
    }

    public void ProfileToMap()
    {
        SceneManager.LoadScene("Map_Scene");
    }
}
