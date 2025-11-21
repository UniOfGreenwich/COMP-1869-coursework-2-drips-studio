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
    public Image bigAvatarImage;   // big image at the top
    public Image[] avatarImages;   // the 3 small images

    [Header("Highlight Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedAvatarIndex = 0;

    private void Start()
    {
        // Set default big image
        if (avatarImages.Length > 0 && bigAvatarImage != null)
        {
            bigAvatarImage.sprite = avatarImages[selectedAvatarIndex].sprite;
        }

        UpdateAvatarHighlights();
    }

    public void OnAvatarClicked(int index)
    {
        selectedAvatarIndex = index;

        // Update highlight
        UpdateAvatarHighlights();

        // Update big image
        if (bigAvatarImage != null && index >= 0 && index < avatarImages.Length)
        {
            bigAvatarImage.sprite = avatarImages[index].sprite;
        }
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
        PlayerProfile.PlayerName = playerNameInput.text;
        PlayerProfile.CafeName = cafeNameInput.text;
        PlayerProfile.AvatarIndex = selectedAvatarIndex;

        SceneManager.LoadScene("NextSceneName"); // change to your real scene name
    }
}
