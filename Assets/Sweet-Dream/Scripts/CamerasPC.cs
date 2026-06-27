using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using MoreMountains.Feedbacks;

public class CamerasPC : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject CreepyBarneyAnimation;
    [SerializeField] private CinemachineCamera cameraPlayer;
    [SerializeField] private CinemachineCamera cameraPC;
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private MMF_Player fadeToBlack;
    [SerializeField] private MMF_Player fadeFromBlack;
    [SerializeField] private float fadeToBlackDuration = 1f;
    [SerializeField] private float fadeFromBlackDuration = 1f;
    [SerializeField] private float viewDuration = 5f;

    private bool isInteracting = false;
    private bool hasBeenUsed = false;

    public void Interact()
    {
        if (isInteracting || hasBeenUsed) return;
        StartCoroutine(PCRoutine());
        if (CreepyBarneyAnimation == null) return;
        CreepyBarneyAnimation.SetActive(true);
    }

    private IEnumerator PCRoutine()
    {
        isInteracting = true;
        hasBeenUsed = true;
        playerInputs.enabled = false;

        fadeToBlack?.PlayFeedbacks();
        yield return new WaitForSeconds(fadeToBlackDuration);

        cameraPlayer.Priority = 0;
        cameraPC.Priority = 20;

        fadeFromBlack?.PlayFeedbacks();
        yield return new WaitForSeconds(fadeFromBlackDuration);

        yield return new WaitForSeconds(viewDuration);

        fadeToBlack?.PlayFeedbacks();
        yield return new WaitForSeconds(fadeToBlackDuration);

        cameraPC.Priority = 0;
        cameraPlayer.Priority = 20;

        fadeFromBlack?.PlayFeedbacks();
        yield return new WaitForSeconds(fadeFromBlackDuration);

        playerInputs.enabled = true;
        isInteracting = false;
    }
}