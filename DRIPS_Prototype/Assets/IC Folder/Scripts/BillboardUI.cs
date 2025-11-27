using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null)
            Debug.LogError("BillboardUI: No main camera found!");
    }

    private void LateUpdate()
    {
        if (mainCam == null) return;

        // Rotate the canvas to face the camera
        transform.LookAt(transform.position + mainCam.transform.forward);
    }
}
