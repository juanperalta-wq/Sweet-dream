using UnityEngine;

// Aparece, se queda visible un momento y luego se reubica en otro punto oculto cercano.
// En su último "salto" tiene una probabilidad de terminar embistiendo (Rusher) en vez
// de desaparecer siempre igual: así el jugador no puede memorizar cuántos parpadeos
// le quedan a un Phantom antes de que sea seguro relajarse.
public class PhantomState : IState
{
    private readonly StateMachine stateMachine;
    private readonly ShadowAI shadow;

    private float visibleTimer;
    private float visibleDuration;
    private int teleportsDone;

    public PhantomState(StateMachine stateMachine, ShadowAI shadow)
    {
        this.stateMachine = stateMachine;
        this.shadow = shadow;
    }

    public void Enter()
    {
        shadow.Agent.isStopped = true;
        visibleTimer = 0f;
        teleportsDone = 0;

        // Jitter: cada Phantom se queda visible un poco más o menos tiempo.
        visibleDuration = shadow.PhantomVisibleDuration * shadow.JitterMultiplier;
    }

    public void Update()
    {
        if (shadow.PlayerTransform != null)
            shadow.RotateTowards(shadow.PlayerTransform.position);

        visibleTimer += Time.deltaTime;

        if (visibleTimer < visibleDuration)
            return;

        teleportsDone++;
        bool isLastTeleport = teleportsDone >= shadow.PhantomMaxTeleports;

        if (isLastTeleport)
        {
            bool canEscalate = !shadow.HasEscalated
                && shadow.IsSafeDistanceToEscalate()
                && Random.value <= shadow.PhantomEscalateChance
                && ShadowManager.Instance != null
                && ShadowManager.Instance.TryConsumeEscalationSlot();

            if (canEscalate)
            {
                shadow.HasEscalated = true;
                stateMachine.ChangeState(shadow.RusherState);
                return;
            }

            shadow.ReturnToPool();
            return;
        }

        if (ShadowManager.Instance == null)
        {
            shadow.ReturnToPool();
            return;
        }

        Transform newSpot = ShadowManager.Instance.RequestReposition(shadow.transform.position);

        if (newSpot == null)
        {
            shadow.ReturnToPool();
            return;
        }

        shadow.Agent.Warp(newSpot.position);
        shadow.transform.rotation = newSpot.rotation;
        visibleTimer = 0f;
    }

    public void Exit() { }
}