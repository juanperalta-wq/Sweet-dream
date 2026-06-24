using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

public class KeyPanel : MonoBehaviour
{
    [BoxGroup("Configuracion")]
    [Required, LabelText("Tag Requerido")]
    [Tooltip("Tag del objeto que activará este panel al entrar al trigger.")]
    [ValidateInput("IsValidTag", "⚠ El tag no puede estar vacío.")]
    public string requiredTag;

    [BoxGroup("Configuracion")]
    [SerializeField, Min(0f), SuffixLabel("s", true)]
    [LabelText("Cooldown")]
    [Tooltip("Tiempo mínimo entre interacciones consecutivas.")]
    private float cooldown = 3f;

    [Space(5)]
    [BoxGroup("Puerta 1")]
    [LabelText("Feedback"), Required]
    public MMF_Player puerta1;

    [BoxGroup("Puerta 2")]
    [LabelText("Feedback"), Required]
    public MMF_Player puerta2;

    private float cooldownTimer = 0f;
    private bool abierta = false;
    private bool feedbacksAreReversed = false;

    public void Interact()
    {
        if (cooldownTimer > 0) return;

        if (!abierta)
        {
            if (feedbacksAreReversed)
            {
                puerta1.ChangeDirection();
                puerta2.ChangeDirection();
                feedbacksAreReversed = false;
            }

            puerta1.PlayFeedbacks();
            puerta2.PlayFeedbacks();
            abierta = true;
        }
        else
        {
            if (!feedbacksAreReversed)
            {
                puerta1.ChangeDirection();
                puerta2.ChangeDirection();
                feedbacksAreReversed = true;
            }

            puerta1.PlayFeedbacks();
            puerta2.PlayFeedbacks();
            abierta = false;
        }

        cooldownTimer = Mathf.Max(puerta1.TotalDuration, puerta2.TotalDuration, cooldown);
    }

    private void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            Interact();
        }
    }

    private bool IsValidTag(string tag) => !string.IsNullOrEmpty(tag);
}