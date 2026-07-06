using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [TabGroup("Feedbacks"), Required]
    public MMF_Player OnOpen;

    [TabGroup("Feedbacks"), Required]
    public MMF_Player OnClose;

    [TabGroup("Feedbacks"), Required]
    public MMF_Player SoundOpen;

    [TabGroup("Feedbacks"), Required]
    public MMF_Player SoundClose;



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
            OnOpen?.PlayFeedbacks();
            SoundOpen?.PlayFeedbacks();

            if (doorNavMesh != null)
                doorNavMesh.OpenDoor();

            abierta = true;
        }
        else
        {
            OnClose?.PlayFeedbacks();
            SoundClose?.PlayFeedbacks();

            if (doorNavMesh != null)
                doorNavMesh.CloseDoor();

            abierta = false;
        }

        cooldownTimer = cooldown;
    }
}