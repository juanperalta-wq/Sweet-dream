using System;
using UnityEngine;

public class MenuInputs : MonoBehaviour
{
    public static event Action<Vector2> OnNavigate;
    public static event Action OnSubmit;
    public static event Action OnCancel;
    public InputSystem_Actions inputs;

    private void Awake()
    {
        inputs = new();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.UI.Escape.performed += ctx => OnCancel?.Invoke();
        inputs.UI.Navigate.performed += ctx => OnNavigate?.Invoke(ctx.ReadValue<Vector2>());
        inputs.UI.Submit.performed += ctx => OnSubmit?.Invoke();
    }

    private void OnDisable()
    {
        inputs.UI.Escape.performed -= ctx => OnCancel?.Invoke();
        inputs.UI.Navigate.performed -= ctx => OnNavigate?.Invoke(ctx.ReadValue<Vector2>());
        inputs.UI.Submit.performed -= ctx => OnSubmit?.Invoke();
        inputs.Disable();
    }
}