using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputs : MonoBehaviour
{
    public static event Action<Vector2> OnNavigate;
    public static event Action OnSubmit;
    public static event Action OnCancel;
    private InputSystem_Actions inputs;

    private void Awake()
    {
        inputs = new();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.UI.Escape.performed += HandleCancel;
        inputs.UI.Navigate.performed += HandleNavigate;
        inputs.UI.Submit.performed += HandleSubmit;
    }

    private void OnDisable()
    {
        inputs.UI.Escape.performed -= HandleCancel;
        inputs.UI.Navigate.performed -= HandleNavigate;
        inputs.UI.Submit.performed -= HandleSubmit;
        inputs.Disable();
    }

    private void HandleCancel(InputAction.CallbackContext ctx) => OnCancel?.Invoke();
    private void HandleNavigate(InputAction.CallbackContext ctx) => OnNavigate?.Invoke(ctx.ReadValue<Vector2>());
    private void HandleSubmit(InputAction.CallbackContext ctx) => OnSubmit?.Invoke();
}