using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteract : MonoBehaviour
{
    [Header("Puerta")]
    public Transform door;

    public Vector3 openPosition;

    private Vector3 closedPosition;

    [Header("Velocidad")]
    public float speed = 2f;

    private bool isOpen = false;
    private bool playerNear = false;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Interact.performed += OnInteract;
    }

    void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Disable();
    }

    void Start()
    {
        closedPosition = door.localPosition;
    }

    void Update()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        door.localPosition = Vector3.Lerp(door.localPosition,targetPosition,speed * Time.deltaTime);
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (playerNear)
        {
            isOpen = !isOpen;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            Debug.Log("Presiona E para abrir");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}
