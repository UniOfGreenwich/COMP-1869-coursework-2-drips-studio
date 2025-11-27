using UnityEngine;

public class CheckWalking : MonoBehaviour
{
    public InteractSquishAnimation squish;
    public GameObject billboardUI;

    [Header("Movement Sensitivity")]
    public float movementThreshold = 0.05f;    // how much movement counts as "walking"

    private Vector3 lastPosition;
    private bool isMoving = false;

    private void Start()
    {
        lastPosition = transform.position;

        if (squish == null)
            Debug.LogError("CheckWalking: ‘squish’ reference missing!");
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = (currentPosition - lastPosition).sqrMagnitude;

        // Check if player moved more than a tiny amount
        bool movingNow = distanceMoved > movementThreshold * movementThreshold;

        if (movingNow != isMoving)  // only update when state changes
        {
            isMoving = movingNow;
            squish.squish = isMoving;
        }

        lastPosition = currentPosition;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Customer"))
        {
            billboardUI.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Customer"))
        {
            billboardUI.SetActive(false);
        }
    }
}
