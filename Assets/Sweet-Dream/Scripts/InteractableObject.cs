using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [TabGroup("Feedbacks"), Required]
    public MMF_Player onOpen;

    [TabGroup("Feedbacks"), Required]
    public MMF_Player onClose;

    [TabGroup("Configuracion")]
    [SerializeField] private float cooldown = 1f;

    private float cooldownTimer = 0f;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private bool abierta = false;

    [SerializeField] private DoorNavMesh doorNavMesh;
    private void Update()
    {
        CoolDown();
    }
    public void CoolDown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }
    public void Interact()
    {
        if (cooldownTimer > 0) return;

        if (!abierta)
        {
            onOpen.PlayFeedbacks();

            if (doorNavMesh != null)
                doorNavMesh.OpenDoor();

            abierta = true;
        }
        else
        {
            onClose.PlayFeedbacks();

            if (doorNavMesh != null)
                doorNavMesh.CloseDoor();

            abierta = false;
        }

        cooldownTimer = cooldown;
    }
}