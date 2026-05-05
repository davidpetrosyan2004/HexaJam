using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;

    public InputAction touchPositionAction;
    public InputAction touchPressAction;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPressAction = playerInput.actions["TouchPress"];
    }

    private void OnEnable()
    {
        touchPressAction.Enable();
        touchPressAction.started += TouchPressed;
    }

    private void OnDisable()
    {
        touchPressAction.started -= TouchPressed;
        touchPressAction.Disable();
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!hit.collider) return;

            Turtle turtle = hit.collider.GetComponent<Turtle>();

            if (turtle != null)
            {
                GameEvents.OnTurtlePressed?.Invoke(turtle);
            }
        }
    }
}