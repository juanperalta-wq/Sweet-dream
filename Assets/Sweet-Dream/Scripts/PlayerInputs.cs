using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public InputSystem_Actions inputs;

    public static event Action<Vector2> OnMoveInputChange;
    public static event Action OnJump;
    public static event Action OnFlashlight;
    public static event Action OnTakePhoto;
    public static event Action OnInteract;
    public static event Action OnInventory;
    public static event Action<bool> OnSprint;
    public static event Action<int> OnSlotSelect;
    public static event Action<float> OnSlotScroll;

    private void Awake()
    {
        inputs = new();
    }
    private void OnEnable()
    {
        inputs.Enable();
        //Slot Scroll
        inputs.Player.SlotScroll.performed += ctx => OnSlotScroll?.Invoke(ctx.ReadValue<Vector2>().y);
        // Slot selection
        inputs.Player.Slot1.performed += ctx => OnSlotSelect?.Invoke(0);
        inputs.Player.Slot2.performed += ctx => OnSlotSelect?.Invoke(1);
        inputs.Player.Slot3.performed += ctx => OnSlotSelect?.Invoke(2);
        inputs.Player.Slot4.performed += ctx => OnSlotSelect?.Invoke(3);
        inputs.Player.Slot5.performed += ctx => OnSlotSelect?.Invoke(4);
        inputs.Player.Slot6.performed += ctx => OnSlotSelect?.Invoke(5);
        // Player actions
        inputs.Player.Move.performed += ctx => OnMoveInputChange?.Invoke(ctx.ReadValue<Vector2>());
        inputs.Player.Move.canceled += ctx => OnMoveInputChange?.Invoke(Vector2.zero);
        inputs.Player.Jump.performed += ctx => OnJump?.Invoke();
        inputs.Player.Interact.performed += ctx => OnInteract?.Invoke();
        inputs.Player.Sprint.started += SprintStarted;
        inputs.Player.Sprint.canceled += SprintCanceled;
        inputs.Player.Inventory.performed += ctx => OnInventory?.Invoke();
        inputs.Player.Flashlight.performed += ctx => OnFlashlight?.Invoke();
        inputs.Player.Photo.performed += ctx => OnTakePhoto?.Invoke();
    }
    private void OnDisable()
    {
        //slot scroll
        inputs.Player.SlotScroll.performed -= ctx => OnSlotScroll?.Invoke(ctx.ReadValue<Vector2>().y);
        // Slot selection
        inputs.Player.Slot1.performed -= ctx => OnSlotSelect?.Invoke(0);
        inputs.Player.Slot2.performed -= ctx => OnSlotSelect?.Invoke(1);
        inputs.Player.Slot3.performed -= ctx => OnSlotSelect?.Invoke(2);
        inputs.Player.Slot4.performed -= ctx => OnSlotSelect?.Invoke(3);
        inputs.Player.Slot5.performed -= ctx => OnSlotSelect?.Invoke(4);
        inputs.Player.Slot6.performed -= ctx => OnSlotSelect?.Invoke(5);
        // General actions
        inputs.Player.Move.performed -= ctx => OnMoveInputChange?.Invoke(ctx.ReadValue<Vector2>());
        inputs.Player.Move.canceled -= ctx => OnMoveInputChange?.Invoke(Vector2.zero);
        inputs.Player.Jump.performed -= ctx => OnJump?.Invoke();
        inputs.Player.Interact.performed -= ctx => OnInteract?.Invoke();
        inputs.Player.Sprint.started -= SprintStarted;
        inputs.Player.Sprint.canceled -= SprintCanceled;
        inputs.Player.Flashlight.performed -= ctx => OnFlashlight?.Invoke();
        inputs.Player.Photo.performed -= ctx => OnTakePhoto?.Invoke();
        inputs.Player.Inventory.performed -= ctx => OnInventory?.Invoke();
        inputs.Disable();
    }

    private void SprintStarted(InputAction.CallbackContext ctx)
    {
        OnSprint?.Invoke(true);
    }

    private void SprintCanceled(InputAction.CallbackContext ctx)
    {
        OnSprint?.Invoke(false);
    }

}
