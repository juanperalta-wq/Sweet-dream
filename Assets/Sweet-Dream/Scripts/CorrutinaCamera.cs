using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CorrutinaCamera : MonoBehaviour
{
    [SerializeField] private float changeTime = 0.5f;
    [SerializeField] private float currentTime;



    public void InitiationCorrutine()
    {
        StartCoroutine(CountForChangeScene());
    }
    public IEnumerator CountForChangeScene()
    {
        Debug.Log("Contando... changeTime: " + changeTime);
        currentTime = 0;

        while (currentTime < changeTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Cargando escena");
        SceneManager.LoadScene("scene_Home");
    }
}
