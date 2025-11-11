using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:

                offset = transform.position - BuildingSystem.GetTouchWorldPosition();

                break;

            case TouchPhase.Moved:
                
                Vector3 position = BuildingSystem.GetTouchWorldPosition() + offset;
                transform.position = BuildingSystem.current.SnapCoordinateToGrid(position);

                break;
        }
    }
}
