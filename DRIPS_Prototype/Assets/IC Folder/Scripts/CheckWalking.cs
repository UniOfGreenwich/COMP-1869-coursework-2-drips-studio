using UnityEngine;

public class CheckWalking : MonoBehaviour
{
    public InteractSquishAnimation squish;

    [Header("Movement Sensitivity")]
    public float movementThreshold = 0.01f;    // how much movement counts as "walking"

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
}
