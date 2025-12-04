using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 movementInput { get; private set; }
    public bool isInteractionPressed { get; private set; }
    public bool interactionJustPressed { get; private set; }  // 눌린 순간
    public bool interactionJustReleased { get; private set; } // 뗀 순간
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnInteractionInput(InputAction.CallbackContext context)
    {
        // 눌린 순간
        if (context.started)
        {
            interactionJustPressed = true;
            isInteractionPressed = true;
        }

        // 눌린 상태 유지
        if (context.performed)
        {
            isInteractionPressed = true;
        }

        // 뗀 순간
        if (context.canceled)
        {
            interactionJustReleased = true;
            isInteractionPressed = false;
        }
    }

    // 매 프레임 업데이트 후 초기화용
    private void LateUpdate()
    {
        interactionJustPressed = false;
        interactionJustReleased = false;
    }
}
