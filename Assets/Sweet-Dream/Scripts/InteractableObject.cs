using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [TabGroup("Feedbacks"), Required]
    public MMFeedbacks onOpen;

    [TabGroup("Feedbacks"), Required]
    public MMFeedbacks onClose;

    [TabGroup("Configuracion")]
    [SerializeField] private float cooldown = 1f;

    private float cooldownTimer = 0f;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private bool abierta = false;

    private void Update()
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
            abierta = true;
        }
        else
        {
            onClose.PlayFeedbacks();
            abierta = false;
        }

        cooldownTimer = cooldown;
    }
}