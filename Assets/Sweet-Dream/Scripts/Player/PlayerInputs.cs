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
    public static event Action OnRemoveItem;

    private void Awake()
    {
        inputs = new();
    }

    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.RemoveItem.performed += HandleRemoveItem;
        inputs.Player.SlotScroll.performed += HandleSlotScroll;

        inputs.Player.Slot1.performed += ctx => OnSlotSelect?.Invoke(0);
        inputs.Player.Slot2.performed += ctx => OnSlotSelect?.Invoke(1);
        inputs.Player.Slot3.performed += ctx => OnSlotSelect?.Invoke(2);
        inputs.Player.Slot4.performed += ctx => OnSlotSelect?.Invoke(3);
        inputs.Player.Slot5.performed += ctx => OnSlotSelect?.Invoke(4);
        inputs.Player.Slot6.performed += ctx => OnSlotSelect?.Invoke(5);

        inputs.Player.Move.performed += HandleMove;
        inputs.Player.Move.canceled += HandleMoveCanceled;
        inputs.Player.Jump.performed += ctx => OnJump?.Invoke();
        inputs.Player.Interact.performed += ctx => OnInteract?.Invoke();
        inputs.Player.Sprint.started += HandleSprintStarted;
        inputs.Player.Sprint.canceled += HandleSprintCanceled;
        inputs.Player.Inventory.performed += ctx => OnInventory?.Invoke();
        inputs.Player.Flashlight.performed += ctx => OnFlashlight?.Invoke();
        inputs.Player.Photo.performed += ctx => OnTakePhoto?.Invoke();
    }

    private void OnDisable()
    {
        inputs.Player.RemoveItem.performed -= HandleRemoveItem;
        inputs.Player.SlotScroll.performed -= HandleSlotScroll;

        inputs.Player.Slot1.performed -= ctx => OnSlotSelect?.Invoke(0);
        inputs.Player.Slot2.performed -= ctx => OnSlotSelect?.Invoke(1);
        inputs.Player.Slot3.performed -= ctx => OnSlotSelect?.Invoke(2);
        inputs.Player.Slot4.performed -= ctx => OnSlotSelect?.Invoke(3);
        inputs.Player.Slot5.performed -= ctx => OnSlotSelect?.Invoke(4);
        inputs.Player.Slot6.performed -= ctx => OnSlotSelect?.Invoke(5);

        inputs.Player.Move.performed -= HandleMove;
        inputs.Player.Move.canceled -= HandleMoveCanceled;
        inputs.Player.Jump.performed -= ctx => OnJump?.Invoke();
        inputs.Player.Interact.performed -= ctx => OnInteract?.Invoke();
        inputs.Player.Sprint.started -= HandleSprintStarted;
        inputs.Player.Sprint.canceled -= HandleSprintCanceled;
        inputs.Player.Inventory.performed -= ctx => OnInventory?.Invoke();
        inputs.Player.Flashlight.performed -= ctx => OnFlashlight?.Invoke();
        inputs.Player.Photo.performed -= ctx => OnTakePhoto?.Invoke();

        inputs.Disable();
    }

    private void HandleRemoveItem(InputAction.CallbackContext ctx) => OnRemoveItem?.Invoke();
    private void HandleSlotScroll(InputAction.CallbackContext ctx) => OnSlotScroll?.Invoke(ctx.ReadValue<Vector2>().y);
    private void HandleMove(InputAction.CallbackContext ctx) => OnMoveInputChange?.Invoke(ctx.ReadValue<Vector2>());
    private void HandleMoveCanceled(InputAction.CallbackContext ctx) => OnMoveInputChange?.Invoke(Vector2.zero);
    private void HandleSprintStarted(InputAction.CallbackContext ctx) => OnSprint?.Invoke(true);
    private void HandleSprintCanceled(InputAction.CallbackContext ctx) => OnSprint?.Invoke(false);
}