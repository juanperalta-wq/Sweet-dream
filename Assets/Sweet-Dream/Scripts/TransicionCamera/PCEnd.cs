using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PCEnd : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject BarneyPerreo;
    [SerializeField] private CinemachineCamera cameraPlayer;
    [SerializeField] private CinemachineCamera cameraEnd;
    [SerializeField] private GameObject cameraObjectEnd;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerAnimation;
    [SerializeField] private GameObject Luz;
    [SerializeField] private float DurationDance = 10f;


    public void Interact()
    {
        StartCoroutine(ChargeCamera());
    }

    private IEnumerator ChargeCamera()
    {
        Player.SetActive(false);
        cameraObjectEnd.SetActive(true);
        cameraPlayer.Priority = 0;
        cameraEnd.Priority = 160;
        PlayerAnimation.SetActive(true);

        // Espera a que termine la animación del jugador
        yield return new WaitForSeconds(10f);

        // Comienza el baile de Barney
        BarneyPerreo.SetActive(true);

        // Espera la duración del baile
        yield return new WaitForSeconds(DurationDance);

        // Carga la escena de victoria
        SceneManager.LoadScene("UI_Victoria");
    }
}
