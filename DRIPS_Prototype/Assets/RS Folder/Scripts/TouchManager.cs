using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private GameObject player;       // The player object that will move
    [SerializeField] private float playerMovSpeed;    // Speed at which the player moves

    private PlayerInput playerInput;                  // Reference to PlayerInput component

    private InputAction touchPositionAction;          // Action to read screen touch position
    private InputAction touchPressAction;             // Action to detect when the screen is pressed

    private Vector3 position;                         // Target world position where player should move
    private bool moving;                              // Flag to check if the player is currently moving

    private void Awake()
    {
        // Get PlayerInput component and cache actions
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            touchPositionAction = playerInput.actions["TouchPosition"];
            touchPressAction = playerInput.actions["TouchPress"];
        }
    }

    private void Update()
    {
        // If player is flagged as moving, move towards the target position
        if (moving)
        {
            // Move player towards target smoothly at defined speed
            player.transform.position = Vector3.MoveTowards(player.transform.position, position, playerMovSpeed * Time.deltaTime);

            // Stop moving when close enough to the target
            if (Vector3.Distance(player.transform.position, position) < 0.01f)
            {
                moving = false;
            }
        }
    }

    private void OnEnable()
    {
        // Subscribe to touch press input
        touchPressAction.performed += TouchPressed;
    }

    private void OnDisable()
    {
        // Unsubscribe when object is disabled
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        // Get the touch position from input (screen space)
        Vector2 touchedPosition = touchPositionAction.ReadValue<Vector2>();

        // Create a ray from the camera through the touched screen point
        Ray ray = Camera.main.ScreenPointToRay(touchedPosition);

        // Define a horizontal plane at the player's Y position
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, player.transform.position.y, 0));

        float enter;
        // If the ray hits the plane, set that point as the new target position
        if (groundPlane.Raycast(ray, out enter))
        {
            position = ray.GetPoint(enter);   // Get the intersection point in world space
            moving = true;                    // Start moving the player
        }
    }
}