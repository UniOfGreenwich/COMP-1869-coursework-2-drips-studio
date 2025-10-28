using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IC_Touchscreen : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent navMeshAgent;
    public Camera mainCamera;
    public Transform cameraRig;
    public GameObject takeButton;
    public GameObject makeButton;
    public GameObject serveButton;

    [Header("Panning Settings")]
    public float panSpeed = 0.02f;
    public float dragThreshold = 10f;

    private Vector2 startTouchPos;
    private bool isDragging = false;

    [SerializeField] List<string> pointOfInterests;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (CheckUI())
                {
                    startTouchPos = touch.position;
                    isDragging = false;
                }
                break;

            case TouchPhase.Moved:
                if (CheckUI())
                {
                    if (Vector2.Distance(touch.position, startTouchPos) > dragThreshold)
                    {
                        isDragging = true;

                        // Convert screen delta to world-space delta
                        Vector2 delta = touch.deltaPosition;

                        // ISOMETRIC TRANSLATION
                        Vector3 right = mainCamera.transform.right;
                        Vector3 forward = mainCamera.transform.forward;
                        right.y = 0;
                        forward.y = 0;
                        right.Normalize();
                        forward.Normalize();

                        // Combine input with camera orientation
                        Vector3 move = (-right * delta.x - forward * delta.y) * panSpeed;

                        // Move camera rig along the ground plane
                        cameraRig.position += move;
                    }
                }
                break;

            case TouchPhase.Ended:
                if (!isDragging)
                {
                    if (CheckUI())
                    {
                        Ray ray = mainCamera.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            //put here check of hit collider with point of interest collider and move close to it

                            for(int i = 0; i < pointOfInterests.Count; i++)
                            if (hit.collider.CompareTag(pointOfInterests[i]))
                            {
                                navMeshAgent.destination = hit.point;
                            }
                        }
                    }
                }
                break;
        }
    }

    private bool CheckUI()   // Check if any buttons are active in the scene
    {
        if (takeButton.activeSelf)
        {
            return false;
        }
        if (makeButton.activeSelf)
        {
            return false;
        }
        if (serveButton.activeSelf)
        {
            return false;
        }
        return true;
    }
}
