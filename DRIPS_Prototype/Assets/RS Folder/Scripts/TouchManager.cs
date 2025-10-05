using UnityEngine;
using UnityEngine.InputSystem;
public class TouchManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float playerMovSpeed;

    private PlayerInput playerInput;

    private InputAction touchPositionAction;
    private Vector3 position;
    private bool moving;

    private InputAction touchPressAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            touchPositionAction = playerInput.actions["TouchPosition"];
            touchPressAction = playerInput.actions["TouchPress"];
        }
    }
    private void Update()
    {
        if (moving)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, position, playerMovSpeed * Time.deltaTime);

            if (Vector3.Distance(player.transform.position, position) < 0.01f)
            {
                moving = false;
            }
        }
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        Vector2 touchedPosition = touchPositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(touchedPosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, player.transform.position.y, 0));
        
        float enter;
        if (groundPlane.Raycast(ray, out enter))
        {
            position = ray.GetPoint(enter);
            moving = true;
        }
    }
}