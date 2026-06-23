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
    private float cooldown = 1f;

    [Space(5)]
    [BoxGroup("Puerta 1")]
    [LabelText("Abrir"), Required]
    public MMFeedbacks onOpen1;

    [BoxGroup("Puerta 1")]
    [LabelText("Cerrar"), Required]
    public MMFeedbacks onClose1;

    [Space(5)]
    [BoxGroup("Puerta 2")]
    [LabelText("Abrir"), Required]
    public MMFeedbacks onOpen2;

    [BoxGroup("Puerta 2")]
    [LabelText("Cerrar"), Required]
    public MMFeedbacks onClose2;

    private float cooldownTimer = 0f;
    private bool abierta = false;

    public void Interact()
    {
        if (cooldownTimer > 0) return;

        if (!abierta)
        {
            onOpen1?.PlayFeedbacks();
            onOpen2?.PlayFeedbacks();
            abierta = true;
        }
        else
        {
            onClose1?.PlayFeedbacks();
            onClose2?.PlayFeedbacks();
            abierta = false;
        }

        cooldownTimer = cooldown;
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
            Debug.Log($"Interacting with {gameObject.name} using {other.gameObject.name}");
            Interact();
        }
    }

    private bool IsValidTag(string tag) => !string.IsNullOrEmpty(tag);
}