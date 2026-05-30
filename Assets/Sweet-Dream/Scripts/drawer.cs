using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class drawer : MonoBehaviour, IInteractable
{
    [TabGroup("Feedbacks"), Required]
    public MMFeedbacks onOpen;

    [TabGroup("Feedbacks"), Required]
    public MMFeedbacks onClose;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private bool abierta = false;

    public void Interact()
    {
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
    }
}
