using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;

    private InputSystem_Actions inputActions;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float xRotation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        GetInput();

        Move();

        Look();
    }

    void GetInput()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        lookInput = inputActions.Player.Look.ReadValue<Vector2>();
    }

    void Move()
    {
        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;

        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotación vertical de cámara
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal del jugador
        transform.Rotate(Vector3.up * mouseX);
    }
}