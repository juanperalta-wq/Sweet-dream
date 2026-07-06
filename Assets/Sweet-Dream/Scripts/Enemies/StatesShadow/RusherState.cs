using UnityEngine;

// Corre directo hacia el jugador. Pensado para tramos de sanidad baja, o como
// escalación desde StalkerState/PhantomState cuando deciden dejar de esconderse.
public class RusherState : IState
{
    private readonly StateMachine stateMachine;
    private readonly ShadowAI shadow;

    private float chaseTimer;
    private float maxChaseTime;

    public RusherState(StateMachine stateMachine, ShadowAI shadow)
    {
        this.stateMachine = stateMachine;
        this.shadow = shadow;
    }

    public void Enter()
    {
        shadow.Agent.speed = shadow.RusherSpeed;
        shadow.Agent.isStopped = false;
        chaseTimer = 0f;

        // Jitter: cada Rusher persiste un poco más o menos tiempo antes de rendirse.
        maxChaseTime = shadow.RusherMaxChaseTime * shadow.JitterMultiplier;

        if (shadow.PlayerTransform != null)
            shadow.Agent.SetDestination(shadow.PlayerTransform.position);
    }

    public void Update()
    {
        chaseTimer += Time.deltaTime;

        if (shadow.PlayerTransform != null)
        {
            if (!shadow.IsPathBlocked)
                shadow.Agent.SetDestination(shadow.PlayerTransform.position);

            // Si una puerta cerrada la tiene atascada, se reubica del otro lado en vez
            // de quedarse ahí para siempre embistiendo el aire.
            if (shadow.TickStuckWatchdog(shadow.PlayerTransform.position))
                shadow.Agent.SetDestination(shadow.PlayerTransform.position);
        }

        if (shadow.DistanceToPlayer() <= shadow.RusherHitRange)
        {
            Scare();
            shadow.ReturnToPool();
            return;
        }

        // Si nunca alcanza al jugador (atascada, puerta cerrada, etc.), se desvanece
        // igual para no quedar corriendo eternamente por el mapa.
        if (chaseTimer >= maxChaseTime)
            shadow.ReturnToPool();
    }

    public void Exit()
    {
        shadow.Agent.ResetPath();
    }

    private void Scare()
    {
        // Reutiliza el mismo daño que ya usa AttackState.DealDamage() para el enemigo
        // principal. Si prefieres que solo asuste (sin quitar una vida), cambia esto
        // por un feedback propio de sanidad en vez de TakeDamage().
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.TakeDamage();
    }
}