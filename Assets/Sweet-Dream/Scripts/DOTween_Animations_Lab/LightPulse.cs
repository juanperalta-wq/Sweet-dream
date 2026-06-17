using UnityEngine;

public class LightPulse : MonoBehaviour
{
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float maxIntensity = 5f;

    private Light myLight;
    private float timer;

    private void Start()
    {
        myLight = GetComponent<Light>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = (timer % duration) / duration;

        myLight.intensity =
            intensityCurve.Evaluate(t) * maxIntensity;
    }
}