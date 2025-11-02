using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class IC_Touchscreen : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent navMeshAgent;
    public Camera mainCamera;
    public Transform cameraRig;

    [Header("Panning Settings")]
    public float panSpeed = 0.02f;
    public float dragThreshold = 10f;

    private Vector2 startTouchPos;
    private bool isDragging = false;

    [SerializeField] List<string> pointOfInterests = new List<string>();

    void Awake()
    {
        UpdatePoI();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        navMeshAgent = GameObject.FindGameObjectWithTag("Player").GetComponent<NavMeshAgent>();
        cameraRig = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<Transform>();
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
                //if (CheckUI())
                //{
                    startTouchPos = touch.position;
                    isDragging = false;
                //}
                break;

            case TouchPhase.Moved:
                if (Input.touchCount == 1)
                {
                    //if (CheckUI())
                    //{
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

                            if (cameraRig != null)
                            {
                                // Move camera rig along the ground plane
                                cameraRig.position += move;
                            }
                        }
                    //}
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
                    {

                        Camera.main.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + 0.5f, 4, 24);
                    }

                    if (touchDeltaDistance + varianceDistance > 1)
                    {

                        Camera.main.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - 0.5f, 4, 24);
                    }
                }
                break;

            case TouchPhase.Ended:
                if (!isDragging)
                {
                    //if (CheckUI())
                    //{
                        Ray ray = mainCamera.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            //Checks through the point of interests (PoI) and once it's matches a known PoI, it moves to that direction
                            for (int i = 0; i < pointOfInterests.Count; i++)
                                if (hit.collider.CompareTag(pointOfInterests[i]))
                                {
                                    if(navMeshAgent != null)
                                    {                                
                                        navMeshAgent.destination = hit.point;
                                    }
                                }
                        }
                    //}
                }
                break;
        }
    }

    public void UpdatePoI()
    {
        List<GameObject> tempObjectInSceneList = GetNonSceneObjects();
        List<string> tempTagList = GetAllTags(tempObjectInSceneList);

        for(int i = 0; i < tempTagList.Count; i++)
        {
            if (tempTagList[i].Contains("PoI"))
            {
                pointOfInterests.Add(tempTagList[i]);
            }
        }
        
    }
    List<GameObject> GetNonSceneObjects()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject go in Object.FindObjectsOfType<GameObject>(true))
        {
            if (!UnityEditor.EditorUtility.IsPersistent(go))
            {
                objectsInScene.Add(go);
            }
        }
        return objectsInScene;
    }

    List<string> GetAllTags(List<GameObject> gameObjectList)
    {
        List<string> tagsInScene = new List<string>();

        foreach (GameObject go in gameObjectList)
        {
            tagsInScene.Add(go.tag);
        }
        return tagsInScene;
    }
}
