using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class PCEnd : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject BarneyPerreo;
    [SerializeField] private CinemachineCamera cameraPlayer;
    [SerializeField] private CinemachineCamera cameraEnd;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerAnimation;
    [SerializeField] private float DurationDance = 10f;


    public void Interact()
    {
        cameraPlayer.Priority = 0;
        cameraEnd.Priority = 100;

        Player.SetActive(false);

        PlayerAnimation.SetActive(true);

        BarneyPerreo.SetActive(true);
    }
}
