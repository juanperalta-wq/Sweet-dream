using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Sirenix.OdinInspector;

public class FirstPersonController : MonoBehaviour
{
    [FoldoutGroup("Referencias")]
    public InputSystem_Actions inputs;
    [FoldoutGroup("Referencias")]
    private CharacterController controller;
    [FoldoutGroup("Referencias")]
    public CinemachineCamera characterCamera;
    [FoldoutGroup("Referencias")]
    private Vector2 moveInput;
    [FoldoutGroup("Movimiento")]
    public float moveSpeed = 5f;
    [FoldoutGroup("Movimiento")]
    public float jumpForce = 10;
    [FoldoutGroup("Movimiento")]
    public float verticalVelocity = 0;
    [FoldoutGroup("Movimiento")]
    public float pushForce = 4;

    private void Awake()
    {
        inputs = new();
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputs.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputs.Player.Jump.performed -= OnJump;
        inputs.Disable();
    }

    void Update()
    {
        OnMove();
    }

    public void OnMove()
    {
        Vector3 cameraForwardDir = characterCamera.transform.forward;
        cameraForwardDir.y = 0;
        cameraForwardDir.Normalize();

        transform.rotation = Quaternion.LookRotation(cameraForwardDir);

        Vector3 moveDir = (cameraForwardDir * moveInput.y + transform.right * moveInput.x) * moveSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        moveDir.y = verticalVelocity;

        controller.Move(moveDir * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded) return;
        verticalVelocity = jumpForce;
    }

    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 pushDir = (hit.transform.position - transform.position).normalized;

        if (hit.rigidbody != null && hit.rigidbody.linearVelocity == Vector3.zero)
            hit.rigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }*/
}