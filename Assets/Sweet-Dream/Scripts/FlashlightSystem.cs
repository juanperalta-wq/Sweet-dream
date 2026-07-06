using MoreMountains.Feedbacks;
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

    [TabGroup("Sound")]
    [Required]
    [SerializeField] private MMF_Player toggleFeedback;

    public MMProgressBar TargetProgressBar;

    [SerializeField] private LayerMask enemyLayer;

    private float battery = 100f;
    private float drainBattery = 1f;
    private bool isOn;
    private Coroutine batteryCoroutine;
    private Coroutine flickerCoroutine;

    public void OnEnable()
    {
        PlayerInputs.OnFlashlight += ToggleFlashlight;

        // Si la linterna ya estaba encendida al equiparla, reanudar el drenaje
        if (isOn && batteryCoroutine == null)
            batteryCoroutine = StartCoroutine(BatteryDrainRoutine());
    }

    private void OnDisable()
    {
        PlayerInputs.OnFlashlight -= ToggleFlashlight;

        // Al desequipar, detener el drenaje
        if (batteryCoroutine != null)
        {
            StopCoroutine(batteryCoroutine);
            batteryCoroutine = null;
        }
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

        toggleFeedback?.PlayFeedbacks();   // se dispara para ambos casos

        UpdateFlickerRoutine();
    }

    private void UpdateFlickerRoutine()
    {
        bool shouldFlicker = isOn && battery <= 50;

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
                flashlight.intensity = 10f;
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
        var shortWait = new WaitForSeconds(0.05f);

        while (true)
        {
            if (Random.value < 0.3f)
            {
                // apagón breve
                flashlight.intensity = 0f;
                yield return shortWait;

                // a veces doble apagón
                if (Random.value < 0.3f)
                    yield return shortWait;
            }
            else
            {
                // intensidad variable, no solo on/off
                flashlight.intensity = Random.Range(0.5f, 10f);
                yield return new WaitForSeconds(Random.Range(0.08f, 0.25f));
            }
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