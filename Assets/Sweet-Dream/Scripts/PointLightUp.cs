using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PointLightUp : MonoBehaviour
{
    [SerializeField] private Light spotLightUp;
    [SerializeField] private MMF_Player lightFailFeedback;

    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 3f;
    [SerializeField] private float flickerInterval = 0.5f;
    [SerializeField] private float sparkInterval = 10f;

    private void Start()
    {
        StartCoroutine(LightFailureRoutine());
        StartCoroutine(SparkRoutine());
    }
    
    private IEnumerator LightFailureRoutine()
    {
        var wait = new WaitForSeconds(flickerInterval);

        while (true)
        {
            bool isFail = Random.value < 0.5f;
            spotLightUp.intensity = isFail ? minIntensity : maxIntensity;
            yield return wait;
        }
    }

    private IEnumerator SparkRoutine()
    {
        var wait = new WaitForSeconds(sparkInterval);

        while (true)
        {
            lightFailFeedback?.PlayFeedbacks();
            yield return wait;
        }
    }
}