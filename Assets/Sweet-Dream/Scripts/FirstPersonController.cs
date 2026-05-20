using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Sirenix.OdinInspector;

public class FirstPersonController : MonoBehaviour
{
    [Title("Referencias")]
    public InputSystem_Actions inputs;
    private CharacterController controller;
    public CinemachineCamera characterCamera;

    [Title("Movimiento")]
    [SuffixLabel("u/s")] public float moveSpeed = 5f;

    [Title("Salto")]
    [SuffixLabel("fuerza")] public float jumpForce = 10;
    [ReadOnly] public float verticalVelocity = 0;

    [Title("Colisiones")]
    [SuffixLabel("fuerza")] public float pushForce = 4;

    [Title("Debug")]
    [ReadOnly, SerializeField] private Vector2 moveInput;

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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 pushDir = (hit.transform.position - transform.position).normalized;

        if (hit.rigidbody != null && hit.rigidbody.linearVelocity == Vector3.zero)
            hit.rigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}