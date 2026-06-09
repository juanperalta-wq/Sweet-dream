using UnityEngine;
using Unity.Cinemachine;
using Sirenix.OdinInspector;

public class FirstPersonController : MonoBehaviour
{
    [TabGroup("Referencias"), Required]
    [SerializeField] private CharacterController controller;

    [TabGroup("Referencias"), Required]
    [SerializeField] private CinemachineCamera characterCamera;

    [TabGroup("Movimiento"), LabelWidth(110)]
    [PropertyRange(0.1f, 20f)]
    [SerializeField] private float sprintSpeed = 10f;
    private float baseMoveSpeed;

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

    [TabGroup("Interaccion"), LabelWidth(110)]
    [SerializeField] private LayerMask Interactable;

    [TabGroup("Interaccion")]
    [SerializeField] private float distancia = 2f;

    public void Awake()
    {
        baseMoveSpeed = moveSpeed;
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnEnable()
    {
        PlayerInputs.OnSprint += HandleSprint;
        PlayerInputs.OnMoveInputChange += HandleMove;
        PlayerInputs.OnJump += HandleJump;
        PlayerInputs.OnInteract += Interact;
    }

    public void OnDisable()
    {
        PlayerInputs.OnMoveInputChange -= HandleMove;
        PlayerInputs.OnJump -= HandleJump;
        PlayerInputs.OnInteract -= Interact;
        PlayerInputs.OnSprint -= HandleSprint;
    }

    public void Update()
    {
        isGrounded = controller.isGrounded;
        Move();
    }

    #region Move
    private void HandleMove(Vector2 input)
    {
        moveInput = input;
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
    #endregion

    #region Jump
    private void HandleJump()
    {
        if (!controller.isGrounded) return;
        verticalVelocity = jumpForce;
    }
    #endregion

    #region Sprint
    private void HandleSprint(bool isHolding)
    {
        if (isHolding)
            moveSpeed = sprintSpeed;
        else
            moveSpeed = baseMoveSpeed;
    }
    #endregion

    #region Collider
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 pushDir = (hit.transform.position - transform.position).normalized;

        if (hit.rigidbody != null && hit.rigidbody.linearVelocity == Vector3.zero)
            hit.rigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (characterCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(characterCamera.transform.position, characterCamera.transform.forward * distancia);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(characterCamera.transform.position + characterCamera.transform.forward * distancia, 0.1f);
        }
    }
    #endregion

    #region Interact
    public void Interact() //esto se puede mejorar con un sistema de eventos, pero por ahora es suficiente para el prototipo
    {
        Ray ray = new Ray(characterCamera.transform.position, characterCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distancia, Interactable))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
    #endregion

    #region Getters
    public float CurrentSpeed => controller.velocity.magnitude;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float PushForce => pushForce;
    public bool IsGrounded => isGrounded;
    #endregion
}