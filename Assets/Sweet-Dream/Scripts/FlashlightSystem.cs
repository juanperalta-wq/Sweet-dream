        using MoreMountains.Tools;
        using Sirenix.OdinInspector;
        using System.Collections;
        using UnityEngine;
        using UnityEngine.Experimental.GlobalIllumination;

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
            [SerializeField]private LayerMask Barney;
            private float battery = 100f;
            private float drainBattery = 1f;
            private bool isOn;
            private Coroutine batteryCorrutine;
            private Coroutine fleckerCoroutine;
            private float randomNumber;

            private void OnEnable()
            {   
                PlayerInputs.OnFlashlight += ToggleFlashlight;
            }
            private void OnDisable()
            {
                PlayerInputs.OnFlashlight -= ToggleFlashlight;
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
                if (battery <= 0 && !isOn) return;
                isOn = !isOn;

                flashlight.enabled = isOn;
                Debug.Log(battery);
            if (isOn)
                batteryCorrutine = StartCoroutine(BatteryDrainRoutine());
            else 
            {
                StopCoroutine(batteryCorrutine);
                FailFleckerRoutine();
            }
            }
            private void FailFleckerRoutine()
            {
                if (isOn && battery <= 90)
                {
                    if (fleckerCoroutine == null)
                        fleckerCoroutine = StartCoroutine(NumberRandom());
                }
                else
                {
                    if (fleckerCoroutine != null)
                    {
                        StopCoroutine(fleckerCoroutine);
                        fleckerCoroutine = null;
                        flashlight.intensity = 3; 
                    }
                }
            }
            private IEnumerator BatteryDrainRoutine()
            {
                while (isOn)
                {
                    yield return new WaitForSeconds(1f);
                    Battery -= drainBattery;
                }
            }
            private IEnumerator NumberRandom()
            {
                while (true)
                {
                    randomNumber = Random.Range(1, 11);
                    if (randomNumber % 2 == 0)
                    {
                        Debug.Log("Número par (Apagar): " + randomNumber);
                        flashlight.intensity = 0;

                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        Debug.Log("Número impar (Encender): " + randomNumber);
                        flashlight.intensity = 3;
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            void DetectEnemy()
            {
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

                if (Physics.Raycast(ray, out RaycastHit hit, flashlightDistance, Barney))
                {
                    EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();

                    if (enemy != null)
                    {
                        enemy.OnFlashlightHit();
                    }
                }
            }
            public float Battery
            {
                get => battery;
                set
                {
                    battery = Mathf.Clamp(value, 0, 100);

                    if(TargetProgressBar != null)
                    TargetProgressBar.UpdateBar(battery, 0f, 100f);
                    FailFleckerRoutine();
                    if (battery <= 0 && isOn)
                        ToggleFlashlight();
                }
            }
        }