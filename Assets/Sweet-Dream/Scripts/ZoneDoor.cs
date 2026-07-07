using UnityEngine;
using MoreMountains.Feedbacks;

public class ZoneDoor : MonoBehaviour
{
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player openDoor;
    [SerializeField] private MMF_Player closeDoor;
    [SerializeField] private MMF_Player songOpen;
    [SerializeField] private MMF_Player songClose;

    [Header("Filtro")]
    [SerializeField] private string playerTag = "Jason";

    [Header("Opciones")]
    [SerializeField] private bool autoClose = false;
    [SerializeField] private float autoCloseDelay = 3f;

    private bool isOpen = false;
    private float cooldownTimer = 0f;
    private float autoCloseTimer = 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (autoClose && isOpen && cooldownTimer <= 0f)
        {
            autoCloseTimer -= Time.deltaTime;
            if (autoCloseTimer <= 0f)
                Close();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (isOpen || cooldownTimer > 0f) return;

        Open();
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (!isOpen || cooldownTimer > 0f) return;

        Close();
    }

    private void Open()
    {
        if (openDoor == null)
        {
            Debug.LogWarning($"ZoneDoor ({name}): OpenDoor no asignado.");
            return;
        }

        openDoor.PlayFeedbacks();
        songOpen?.PlayFeedbacks();
        isOpen = true;
        cooldownTimer = Mathf.Max(openDoor.TotalDuration, songOpen != null ? songOpen.TotalDuration : 0f);

        if (autoClose)
            autoCloseTimer = autoCloseDelay;
    }

    private void Close()
    {
        if (closeDoor == null)
        {
            Debug.LogWarning($"ZoneDoor ({name}): CloseDoor no asignado.");
            isOpen = false;
            return;
        }

        closeDoor.PlayFeedbacks();
        songClose?.PlayFeedbacks();
        isOpen = false;
        cooldownTimer = Mathf.Max(closeDoor.TotalDuration, songClose != null ? songClose.TotalDuration : 0f);
        autoCloseTimer = 0f;
    }
}