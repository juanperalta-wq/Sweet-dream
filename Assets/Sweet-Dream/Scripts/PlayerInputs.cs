using System;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public InputSystem_Actions inputs;

    public static event Action<Vector2> OnMoveInputChange;
    public static event Action OnJump;
    public static event Action OnFlashlight;
    private void Awake()
    {
        inputs = new();
    }
    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += ctx => OnMoveInputChange?.Invoke(ctx.ReadValue<Vector2>());
        inputs.Player.Move.canceled += ctx => OnMoveInputChange?.Invoke(Vector2.zero);
        inputs.Player.Jump.performed += ctx => OnJump?.Invoke();

        inputs.Player.Flashlight.performed += ctx => OnFlashlight?.Invoke();
    }
    private void OnDisable()
    {
        inputs.Player.Move.performed -= ctx => OnMoveInputChange?.Invoke(ctx.ReadValue<Vector2>());
        inputs.Player.Move.canceled -= ctx => OnMoveInputChange?.Invoke(Vector2.zero);
        inputs.Player.Jump.performed -= ctx => OnJump?.Invoke();
        inputs.Player.Flashlight.performed -= ctx => OnFlashlight?.Invoke();
        inputs.Disable();
    }

}