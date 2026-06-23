using System.Collections;
using UnityEngine;

public class PointLightUp : MonoBehaviour
{
    public Light SpotLightUp;
    private int randomNumber;

    private void Start()
    {

        StartCoroutine(LightFailureRoutine());
    }

    
    private IEnumerator LightFailureRoutine()
    {
        while (true)
        {
            randomNumber = Random.Range(1, 11); 

            if (randomNumber % 2 == 0)
            {
                Debug.Log("Número par (Apagar): " + randomNumber);
                SpotLightUp.intensity = 0;

                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                Debug.Log("Número impar (Encender): " + randomNumber);
                SpotLightUp.intensity = 3;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
