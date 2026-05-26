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
    private bool isOn;


    private void OnEnable()
    {
        PlayerInputs.OnFlashlight += ToggleFlashlight;
    }

    void Update()
    {
        if (isOn)
        {
            DetectEnemy();
        }
    }

    void ToggleFlashlight()
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