using UnityEngine;

// Persigue lentamente manteniéndose detrás/en la periferia del jugador, en vez de ir
// directo hacia él. Si el jugador la sorprende mirándola de frente demasiado tiempo,
// se desvanece (refuerza "algo que se esconde en cuanto lo ves"). Pero si se queda
// demasiado cerca por un rato sostenido, tiene una probabilidad de escalar a Rusher
// en pleno acecho, en vez de desvanecerse siempre igual: eso rompe la previsibilidad.
public class StalkerState : IState
{
    private readonly StateMachine stateMachine;
    private readonly ShadowAI shadow;

    private const float DestinationUpdateInterval = 0.5f;
    private const float MaxExposedTime = 2f;

    private float destinationTimer;
    private float exposedTimer;
    private float tooCloseTimer;
    private float escalateAfter;

    public StalkerState(StateMachine stateMachine, ShadowAI shadow)
    {
        this.stateMachine = stateMachine;
        this.shadow = shadow;
    }

    public void Enter()
    {
        shadow.Agent.speed = shadow.StalkerSpeed;
        shadow.Agent.isStopped = false;
        destinationTimer = 0f;
        exposedTimer = 0f;
        tooCloseTimer = 0f;

        escalateAfter = shadow.StalkerEscalateAfter * shadow.JitterMultiplier;

        UpdateDestination();
    }

    public void Update()
    {
        destinationTimer += Time.deltaTime;

        if (!shadow.IsPathBlocked && destinationTimer >= DestinationUpdateInterval)
        {
            UpdateDestination();
            destinationTimer = 0f;
        }

        if (shadow.PlayerTransform != null && shadow.TickStuckWatchdog(shadow.PlayerTransform.position))
        {
            UpdateDestination();
            return;
        }

        exposedTimer = shadow.IsSeenByPlayer() ? exposedTimer + Time.deltaTime : 0f;

        bool tooClose = shadow.DistanceToPlayer() <= shadow.StalkerTooCloseDistance;
        tooCloseTimer = tooClose ? tooCloseTimer + Time.deltaTime : 0f;

        bool tooExposed = exposedTimer >= MaxExposedTime;

        // Vista de frente sostenidamente: siempre se desvanece (mecánica de sigilo fallido).
        if (tooExposed)
        {
            shadow.ReturnToPool();
            return;
        }
        if (tooCloseTimer >= escalateAfter)
        {
            bool canEscalate = !shadow.HasEscalated
                && Random.value <= shadow.StalkerEscalateChance
                && ShadowManager.Instance != null
                && ShadowManager.Instance.TryConsumeEscalationSlot();

            if (canEscalate)
            {
                shadow.HasEscalated = true;
                stateMachine.ChangeState(shadow.RusherState);
                return;
            }
            shadow.ReturnToPool();
        }
    }

    public void Exit()
    {
        shadow.Agent.ResetPath();
    }
    private void UpdateDestination()
    {
        if (shadow.PlayerTransform == null) return;

        Vector3 behindPlayer = shadow.PlayerTransform.position
            - shadow.PlayerTransform.forward * shadow.StalkerFollowDistance;

        shadow.Agent.SetDestination(behindPlayer);
    }
}