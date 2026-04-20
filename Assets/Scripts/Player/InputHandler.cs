using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct InputData
{
    public Vector2 Move;
    public Vector2 Rotate;
    public bool JumpPressed;
    public bool DashPressed;
    public bool JumpHeld;
    public bool DashHeld;
}

public class InputHandler : MonoBehaviour
{
    public InputData CurrentInput;

    public Vector2 Move => CurrentInput.Move;
    public Vector2 Rotate => CurrentInput.Rotate;
    public bool JumpPressed => CurrentInput.JumpPressed;
    public bool DashPressed => CurrentInput.DashPressed;
    public bool JumpHeld => CurrentInput.JumpHeld;

    public bool DashHeld => CurrentInput.DashHeld;

    private PlayerInputActions InputActions;

    private void Awake()
    {
        InputActions = new PlayerInputActions();
        CurrentInput = new InputData();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        InputActions.Enable();

        InputActions.Player.Move.performed += OnMove;
        InputActions.Player.Move.canceled += OnMove;

        InputActions.Player.Rotate.performed += OnRotate;
        InputActions.Player.Rotate.canceled += OnRotate;

        InputActions.Player.Jump.started += OnJump;
        InputActions.Player.Jump.canceled += OnJump;

        InputActions.Player.Dash.started += OnDash;
        InputActions.Player.Dash.canceled += OnDash;

        //InputActions.Player.Attack.started +=
    }

    private void OnDisable()
    {
        InputActions.Player.Move.performed -= OnMove;
        InputActions.Player.Move.canceled -= OnMove;

        InputActions.Player.Rotate.performed -= OnRotate;
        InputActions.Player.Rotate.canceled -= OnRotate;

        InputActions.Player.Jump.started -= OnJump;
        InputActions.Player.Jump.canceled -= OnJump;

        InputActions.Player.Dash.started -= OnDash;
        InputActions.Player.Dash.canceled -= OnDash;

        InputActions.Disable();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        CurrentInput.Move = ctx.ReadValue<Vector2>();
    }
    public void OnRotate(InputAction.CallbackContext ctx)
    {
        CurrentInput.Rotate = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            CurrentInput.JumpPressed = true;
            CurrentInput.JumpHeld = true;
        }
        if (ctx.canceled)
        {
            CurrentInput.JumpHeld = false;
        }
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            CurrentInput.DashPressed = true;
            CurrentInput.DashHeld = true;
        }
        else
        {
            CurrentInput.DashPressed = false;
        }
    }

    public void ClearJump()
    {
        CurrentInput.JumpPressed = false;
    }

    public void ClearDash()
    {
        CurrentInput.DashPressed = false;
    }
}
