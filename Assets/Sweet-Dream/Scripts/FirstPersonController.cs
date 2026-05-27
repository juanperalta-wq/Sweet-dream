using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Sirenix.OdinInspector;

public class FirstPersonController : MonoBehaviour
{
    [TabGroup("Referencias"), Required]
    [SerializeField] private CharacterController controller;

    [TabGroup("Referencias"), Required]
    [SerializeField] private CinemachineCamera characterCamera;

    [TabGroup("Movimiento"), LabelWidth(110)]
    [PropertyRange(0.1f, 10f)]
    [SerializeField] private float moveSpeed = 5f;

    [TabGroup("Movimiento"), LabelWidth(110)]
    [PropertyRange(0.1f, 20f)]
    [SerializeField] private float jumpForce = 10f;

    [TabGroup("Movimiento"), LabelWidth(110)]
    [Range(1, 10)]
    [SerializeField] private float pushForce = 4f;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private Vector2 moveInput;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private float verticalVelocity;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private bool isGrounded;

    private InputSystem_Actions inputs;

    public void Awake()
    {
        inputs = new();
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputs.Player.Jump.performed += OnJump;
    }

    public void OnDisable()
    {
        inputs.Player.Jump.performed -= OnJump;
        inputs.Disable();
    }

    public void Update()
    {
        isGrounded = controller.isGrounded;
        Move();
    }

    public void Move()
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded) return;
        verticalVelocity = jumpForce;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 pushDir = (hit.transform.position - transform.position).normalized;

        if (hit.rigidbody != null && hit.rigidbody.linearVelocity == Vector3.zero)
            hit.rigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}