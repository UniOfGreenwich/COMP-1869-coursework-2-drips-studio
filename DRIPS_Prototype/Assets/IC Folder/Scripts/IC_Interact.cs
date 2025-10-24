using UnityEngine;
using UnityEngine.UI;

public class IC_Interact : MonoBehaviour
{
    [SerializeField] Image popUpImage;
    private bool isActive;

    void OnMouseDown()
    {
        isActive = !isActive;
        popUpImage.gameObject.SetActive(isActive);

        Debug.Log("Tapped object - " + (isActive ? "ON" : "OFF"));
    }
}
