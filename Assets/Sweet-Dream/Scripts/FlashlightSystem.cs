using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

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

    public MMProgressBar TargetProgressBar;

    [SerializeField] private LayerMask enemyLayer;

    private float battery = 100f;
    private float drainBattery = 1f;
    private bool isOn;
    private Coroutine batteryCoroutine;
    private Coroutine flickerCoroutine;

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
        if (isOn)
            DetectEnemy();
    }

    private void ToggleFlashlight()
    {
        if (battery <= 0 && !isOn) return;

        isOn = !isOn;
        flashlight.enabled = isOn;

        if (isOn)
        {
            batteryCoroutine = StartCoroutine(BatteryDrainRoutine());
        }
        else
        {
            if (batteryCoroutine != null)
            {
                StopCoroutine(batteryCoroutine);
                batteryCoroutine = null;
            }
        }

        UpdateFlickerRoutine();
    }

    private void UpdateFlickerRoutine()
    {
        bool shouldFlicker = isOn && battery <= 90;

        if (shouldFlicker)
        {
            if (flickerCoroutine == null)
                flickerCoroutine = StartCoroutine(FlickerCoroutine());
        }
        else
        {
            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
                flickerCoroutine = null;
            }

            if (isOn)
                flashlight.intensity = 3f;
        }
    }
    public void Recharge(float amount)
    {
        Battery += amount;
    }
    private IEnumerator BatteryDrainRoutine()
    {
        while (isOn)
        {
            yield return new WaitForSeconds(1f);
            Battery -= drainBattery;
        }
    }

    private IEnumerator FlickerCoroutine()
    {
        while (true)
        {
            flashlight.intensity = (Random.value > 0.5f) ? 3f : 0f;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        }
    }

    private void DetectEnemy()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, flashlightDistance, enemyLayer))
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
                enemy.OnFlashlightHit();
        }
    }

    public float Battery
    {
        get => battery;
        set
        {
            battery = Mathf.Clamp(value, 0f, 100f);

            if (TargetProgressBar != null)
                TargetProgressBar.UpdateBar(battery, 0f, 100f);

            UpdateFlickerRoutine();

            if (battery <= 0 && isOn)
                ToggleFlashlight();
        }
    }
}