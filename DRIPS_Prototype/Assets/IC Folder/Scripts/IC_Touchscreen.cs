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
                if (Input.touchCount == 1)
                {
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
                }
                else if (Input.touchCount == 2)
                {
                    Vector2 currentTouch0Pos = Input.GetTouch(0).position;
                    Vector2 currentTouch1Pos = Input.GetTouch(1).position;
                    Vector2 previousTouch0Pos = Input.GetTouch(0).deltaPosition;
                    Vector2 previousTouch1Pos = Input.GetTouch(1).deltaPosition;
                    float currentDistance = Vector2.Distance(currentTouch0Pos, currentTouch1Pos);
                    float previousDistance = Vector2.Distance(currentTouch0Pos - previousTouch0Pos, currentTouch1Pos - previousTouch1Pos);
                    float touchDeltaDistance = currentDistance - previousDistance;
                    float varianceDistance = 5.0f;

                    //Debug.Log("current touch 0: " + currentTouch0Pos);
                    //Debug.Log("previous touch 0: " + previousTouch0Pos);
                    //Debug.Log("current touch 1: " + currentTouch1Pos);
                    //Debug.Log("previous touch 1: " + previousTouch1Pos);
                    //Debug.Log("current distance: " + currentDistance);
                    //Debug.Log("previous distance: " + previousDistance);

                    if (touchDeltaDistance + varianceDistance <= 1)
                    {

                        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 0.1f, 4, 24);
                    }

                    if (touchDeltaDistance + varianceDistance > 1)
                    {

                        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 0.1f, 4, 24);
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
                            //Checks through the point of interests (PoI) and once it's matches a known PoI, it moves to that direction
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
