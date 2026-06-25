using UnityEngine;
using Sirenix.OdinInspector;

public class FlashlightSystem : MonoBehaviour
{
    [TabGroup("Light"), Required]
    [SerializeField] private Light flashlight;

    [TabGroup("Light"), Range(1f, 20f)]
    [SerializeField] private float flashlightDistance = 12f;

    [TabGroup("Light"), Range(0f, 100f)]
    [SerializeField] private float batteryDrainRate = 2f;

    [TabGroup("Battery"), Range(0f, 100f)]
    [SerializeField] private float battery = 100f;

    [TabGroup("Battery"), Range(0f, 100f)]
    [SerializeField]
    private float maxBattery = 100f;

    private float nextFlickerTime;

    [TabGroup("Camera"), Required]
    [SerializeField]
    private Camera playerCamera;

    [TabGroup("Detection")]
    [SerializeField] private LayerMask enemyLayers;

    public float BatteryNormalized => battery / maxBattery;
    public bool IsOn => isOn;
    private bool isOn;
    private float nextDetectTime;

    private void OnEnable() 
    { 
        PlayerInputs.OnFlashlight += ToggleFlashlight;
    }
    private void OnDisable() 
    { 
        PlayerInputs.OnFlashlight -= ToggleFlashlight; 
    }

    private void Update()
    {
        if (!isOn) return;
        DrainBattery();

        if (battery <= 70f && Time.time >= nextFlickerTime)
        {
            flashlight.intensity = Random.Range(0.5f, 1.5f);
            nextFlickerTime = Time.time + Random.Range(0.05f, 0.2f);
        }

        if (Time.time >= nextDetectTime)
        {
            DetectEnemy();
            nextDetectTime = Time.time + 0.1f;
        }
    }

    public void Recharge(float amount)
    {
        battery = Mathf.Min(battery + amount, maxBattery);
    }
    private void ToggleFlashlight()
    {
        if (!isOn && battery <= 0f) return;
        SetFlashlight(!isOn);
    }

    private void SetFlashlight(bool state)
    {
        isOn = state;
        if (flashlight != null) flashlight.enabled = state;
    }

    private void DrainBattery()
    {
        battery -= Time.deltaTime * batteryDrainRate;
        if (battery <= 0f)
        {
            battery = 0f;
            SetFlashlight(false);
        }
    }
    
    private void Flicker()
    {
        if (battery <= 70f)
        {
            flashlight.intensity = Random.Range(0.5f, 1.5f);
        }
    }
    private void DetectEnemy()
    {
        if (playerCamera == null) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, flashlightDistance, enemyLayers))
        {
            hit.collider.GetComponent<EnemyBase>()?.OnFlashlightHit();
        }
    }
}