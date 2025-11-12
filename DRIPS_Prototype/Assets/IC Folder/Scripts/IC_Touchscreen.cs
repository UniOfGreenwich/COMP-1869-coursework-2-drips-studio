using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IC_Touchscreen : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent navMeshAgent;
    public Camera mainCamera;
    public Transform cameraRig;
    public GameObject touchParticleEffect;

    [Header("Panning Settings")]
    public float panSpeed = 0.02f;
    public float dragThreshold = 10f;

    private Vector2 startTouchPos;
    private bool isDragging = false;

    [SerializeField] private List<string> pointOfInterests = new List<string>();

    void Awake()
    {
        UpdatePoI();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (navMeshAgent == null)
            navMeshAgent = GameObject.FindGameObjectWithTag("Player")?.GetComponent<NavMeshAgent>()
                ?? FindAnyObjectByType<NavMeshAgent>();

        if (cameraRig == null)
            cameraRig = GameObject.FindGameObjectWithTag("CameraRig")?.transform
                ?? FindAnyObjectByType<Transform>();
    }

    void Update()
    {
        TouchInput();
    }

    void TouchInput()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPos = touch.position;
                isDragging = false;
                break;

            case TouchPhase.Moved:
                if (Input.touchCount == 1)
                {
                    if (Vector2.Distance(touch.position, startTouchPos) > dragThreshold)
                    {
                        isDragging = true;

                        Vector2 delta = touch.deltaPosition;
                        Vector3 right = mainCamera.transform.right;
                        Vector3 forward = mainCamera.transform.forward;
                        right.y = 0;
                        forward.y = 0;
                        right.Normalize();
                        forward.Normalize();

                        Vector3 move = (-right * delta.x - forward * delta.y) * panSpeed;
                        if (cameraRig != null)
                            cameraRig.position += move;
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

                    if (touchDeltaDistance + varianceDistance <= 1)
                        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + 0.5f, 4, 24);

                    if (touchDeltaDistance + varianceDistance > 1)
                        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - 0.5f, 4, 24);
                }
                break;

            case TouchPhase.Ended:
                if (!isDragging)
                {
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        for (int i = 0; i < pointOfInterests.Count; i++)
                        {
                            if (hit.collider.CompareTag(pointOfInterests[i]))
                            {
                                if (navMeshAgent != null)
                                {
                                    Transform targetPos = hit.collider.transform.Find("Position");

                                    if (targetPos != null)
                                    {
                                        navMeshAgent.destination = targetPos.position;
                                        GameObject pe = Instantiate(touchParticleEffect, targetPos.position, Quaternion.identity);
                                        pe.GetComponent<ParticleSystem>().Play();
                                        Destroy(pe, 1f);
                                    }
                                    else
                                    {
                                        navMeshAgent.destination = hit.point;
                                        GameObject pe = Instantiate(touchParticleEffect, hit.point, Quaternion.identity);
                                        pe.GetComponent<ParticleSystem>().Play();
                                        Destroy(pe, 1f);
                                        Debug.Log($"{hit.collider.name} has no child named 'Position' — using clicked point instead.");
                                    }
                                }
                            }
                        }
                    }
                }
                break;
        }
    }

    public void UpdatePoI()
    {
        pointOfInterests.Clear();

#if UNITY_2023_1_OR_NEWER
        GameObject[] foundObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
#else
        GameObject[] foundObjects = Object.FindObjectsOfType<GameObject>(true);
#endif

        foreach (GameObject go in foundObjects)
        {
            // Only consider tagged scene objects with "PoI" in the tag
            if (go.CompareTag("Untagged") == false && go.tag.Contains("PoI") && !pointOfInterests.Contains(go.tag))
            {
                pointOfInterests.Add(go.tag);
            }
        }

        Debug.Log($"[IC_Touchscreen] Found {pointOfInterests.Count} Points of Interest.");
    }
}
