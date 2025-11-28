using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransfer : MonoBehaviour
{
    public void MapToGame()
    {
        StartCoroutine(MapToGameCoroutine());
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

    IEnumerator MapToGameCoroutine()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Prototype Scene");
        yield return null;
    }
}
