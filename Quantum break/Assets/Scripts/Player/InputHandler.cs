using UnityEngine;
using UnityEngine.InputSystem;

public struct InputData
{
    public Vector2 move;
    public bool jumpPressed;
    public bool dashPressed;
}

public class InputHandler : MonoBehaviour, IInput
{
    public Vector2 Move { get; private set; }

    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }

    public void ClearFrameInput()
    {
        JumpPressed = false;
        DashPressed = false;
    }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Jump.started += OnJump;
        inputActions.Player.Dash.started += OnDash;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.started -= OnJump;
        inputActions.Player.Dash.started -= OnDash;

        inputActions.Disable();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Move = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            JumpPressed = true;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            DashPressed = true;
    }

    public void ClearOneFrameInput()
    {
        JumpPressed = false;
        DashPressed = false;
    }
}
