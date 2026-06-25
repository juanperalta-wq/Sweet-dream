using UnityEngine;
using Sirenix.OdinInspector;
using MoreMountains.Tools;

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
    [SerializeField] private float maxBattery = 100f;

    [TabGroup("Camera"), Required]
    [SerializeField] private Camera playerCamera;

    [TabGroup("Detection")]
    [SerializeField] private LayerMask enemyLayers;

    public MMProgressBar TargetProgressBar;

    public bool IsOn => isOn;

    private bool isOn;
    private float nextDetectTime;
    private float nextFlickerTime;

    private void OnEnable()
    { 
        PlayerInputs.OnFlashlight += ToggleFlashlight;
    }
    private void OnDisable()
    { 
        PlayerInputs.OnFlashlight -= ToggleFlashlight;
    }
    private void FixedUpdate()
    {
        TargetProgressBar?.UpdateBar(battery, 0f, maxBattery);
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
        Battery += amount;
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
        Battery -= Time.deltaTime * batteryDrainRate;
        if (battery <= 0f)
            SetFlashlight(false);
    }

    private void DetectEnemy()
    {
        if (playerCamera == null) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, flashlightDistance, enemyLayers))
            hit.collider.GetComponent<EnemyBase>()?.OnFlashlightHit();
    }
    #region getters and setters
    public float Battery
    {
        get => battery;
        set
        {
            battery = Mathf.Clamp(value, 0f, maxBattery);
            TargetProgressBar?.UpdateBar(battery, 0f, maxBattery);
        }
    }
    #endregion
}