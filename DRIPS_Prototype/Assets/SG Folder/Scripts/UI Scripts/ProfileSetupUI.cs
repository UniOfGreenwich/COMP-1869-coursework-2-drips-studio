using UnityEngine;
using UnityEngine.UI;
using TMPro;            
using UnityEngine.SceneManagement;

public class ProfileSetupUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField playerNameInput;
    public TMP_InputField cafeNameInput;

    [Header("Avatar Options")]
    public Image[] avatarImages;   // The images shown in the UI
    public int selectedAvatarIndex = 0;

    [Header("Highlight Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private void Start()
    {
        UpdateAvatarHighlights();
    }

    // Hook this to each avatar button's OnClick, and pass the index
    public void OnAvatarClicked(int index)
    {
        selectedAvatarIndex = index;
        UpdateAvatarHighlights();
    }

    private void UpdateAvatarHighlights()
    {
        for (int i = 0; i < avatarImages.Length; i++)
        {
            avatarImages[i].color = (i == selectedAvatarIndex) ? selectedColor : normalColor;
        }
    }

    public void OnContinueButton()
    {
        // Save data into static profile
        PlayerProfile.PlayerName = playerNameInput.text;
        PlayerProfile.CafeName = cafeNameInput.text;
        PlayerProfile.AvatarIndex = selectedAvatarIndex;

        // Load next scene (change name to real scene)
        SceneManager.LoadScene("NextSceneName");
    }
}
