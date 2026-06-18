using MoreMountains.Feedbacks;
  using Sirenix.OdinInspector;
  using UnityEngine;

  public class KeyPanel : MonoBehaviour
  {
      [TabGroup("Feedbacks"), Required]
      [LabelText("Tag Requerido")]
      [Tooltip("Tag del objeto que activará este panel al entrar al trigger.")]
      [ValidateInput("IsValidTag", "⚠ El tag no puede estar vacío.")]
      public string requiredTag;

      [TabGroup("Feedbacks"), Required]
      [LabelText("Feedback Abrir")]
      public MMFeedbacks onOpen;

      [TabGroup("Feedbacks"), Required]
      [LabelText("Feedback Cerrar")]
      public MMFeedbacks onClose;

      [TabGroup("Configuracion")]
      [SerializeField, Min(0f), SuffixLabel("s", true)]
      [LabelText("Cooldown")]
      [Tooltip("Tiempo mínimo entre interacciones consecutivas.")]
      private float cooldown = 1f;

      private float cooldownTimer = 0f;

      [TabGroup("Debug"), ReadOnly, ShowInInspector]
      [LabelText("¿Abierta?")]
      [SerializeField] private bool abierta = false;

      [TabGroup("Debug"), ReadOnly, ShowInInspector, ProgressBar(0, 5, ColorGetter = "GetCooldownColor")]
      [LabelText("Cooldown Restante")]
      private float CooldownDisplay => Mathf.Max(0f, cooldownTimer);

      [TabGroup("Debug"), ReadOnly, ShowInInspector]
      [LabelText("Estado")]
      private string Estado => abierta ? "ABIERTA" : "CERRADA";

      [TabGroup("Debug"), Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1f)]
      [LabelText("Probar Interacción")]
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

      [TabGroup("Debug"), Button(ButtonSizes.Medium), GUIColor(1f, 0.5f, 0.5f)]
      [LabelText("Forzar Reset")]
      private void ResetState()
      {
          abierta = false;
          cooldownTimer = 0f;
      }

      private void Update()
      {
          CoolDown();
      }

      public void CoolDown()
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

      private bool IsValidTag(string tag)
      {
          return !string.IsNullOrEmpty(tag);
      }

      private Color GetCooldownColor()
      {
          float pct = cooldownTimer / Mathf.Max(0.01f, cooldown);
          return pct > 0.5f ? new Color(1f, 0.3f, 0.3f) : new Color(0.3f, 1f, 0.3f);
      }
  }