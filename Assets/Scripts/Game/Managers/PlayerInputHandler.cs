using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;

    public InputAction touchPositionAction;
    public InputAction touchPressAction;
    public Turtle tutorialTurtle;
    public bool isTutorial = true;
    private void Awake()
    {
        Application.targetFrameRate = 60;
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
        AudioManager.Instance.Vibrate();
        Vector2 screenPos = touchPositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            Turtle turtle = hit.collider.GetComponent<Turtle>();
            if (turtle == null) return;
            if (isTutorial && turtle != tutorialTurtle)
                return;
            if (hit.collider.CompareTag("Swapper"))
            {
                hit.collider.GetComponent<TurtleSwapper>().Swap();
                Debug.Log("Swaping..");
                return;
            }
            if (!hit.collider.CompareTag("Player")) return;


            GameEvents.OnTurtlePressed?.Invoke(turtle);
        }
    }
}