using MoreMountains.Feedbacks;
using UnityEngine;

public class ZoneCharger : MonoBehaviour
{
    public MMF_Player BaseCharger;
    private bool isOpen = false;
    private bool isReversed = false;
    private float cooldownTimer = 0f;

    private void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen && cooldownTimer <= 0)
        {
            if (isReversed)
            {
                BaseCharger.ChangeDirection();
                isReversed = false;
            }

            BaseCharger.PlayFeedbacks();
            isOpen = true;
            cooldownTimer = BaseCharger.TotalDuration;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen && cooldownTimer <= 0)
        {
            if (!isReversed)
            {
                BaseCharger.ChangeDirection();
                isReversed = true;
            }

            BaseCharger.PlayFeedbacks();
            isOpen = false;
            cooldownTimer = BaseCharger.TotalDuration;
        }
    }
}