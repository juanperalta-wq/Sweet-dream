using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
public class FlashlightSystem : MonoBehaviour
{
    [TabGroup("Light")]
    [Required]
    [SerializeField] private Light flashlight;
    [TabGroup("Camera")]
    [Required]  
    [SerializeField] private Camera playerCamera;
    [TabGroup("Light")]
    [Range(1, 20)]
    [SerializeField] private float flashlightDistance = 12f;
    private InputSystem_Actions inputs;
    private bool isOn;

    private void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.Flashlight.performed += ToggleFlashlight;
    }

    private void OnDisable()
    {
        inputs.Player.Flashlight.performed -= ToggleFlashlight;

        inputs.Disable();
    }

    void Update()
    {
        if (isOn)
        {
            DetectEnemy();
        }
    }

    void ToggleFlashlight(InputAction.CallbackContext context)
    {
        isOn = !isOn;

        flashlight.enabled = isOn;
    }

    void DetectEnemy()
    {
        Ray ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, flashlightDistance))
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.OnFlashlightHit();
            }
        }
    }
}