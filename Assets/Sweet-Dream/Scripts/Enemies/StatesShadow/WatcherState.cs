using UnityEngine;

// Comportamiento pasivo: la sombra se queda inmóvil observando al jugador.
// Desaparece si el jugador la mira fijo demasiado tiempo, si se acerca mucho,
// o simplemente al agotarse su tiempo de vida (para que no se quede ahí para
// siempre si el jugador nunca la nota).
public class WatcherState : IState
{
    private readonly StateMachine stateMachine;
    private readonly ShadowAI shadow;

    private float watchedTimer;
    private float lifeTimer;
    private float maxWatchTime;
    private float lifeTime;

    public WatcherState(StateMachine stateMachine, ShadowAI shadow)
    {
        this.stateMachine = stateMachine;
        this.shadow = shadow;
    }

    public void Enter()
    {
        shadow.Agent.isStopped = true;
        watchedTimer = 0f;
        lifeTimer = 0f;

        // Jitter: cada Watcher aguanta un poco más o menos tiempo antes de desvanecerse.
        maxWatchTime = shadow.WatcherMaxWatchTime * shadow.JitterMultiplier;
        lifeTime = shadow.WatcherLifeTime * shadow.JitterMultiplier;
    }

    public void Update()
    {
        lifeTimer += Time.deltaTime;

        if (shadow.PlayerTransform != null)
            shadow.RotateTowards(shadow.PlayerTransform.position);

        // Solo cuenta la mirada SOSTENIDA: si el jugador deja de verla, se resetea.
        watchedTimer = shadow.IsSeenByPlayer() ? watchedTimer + Time.deltaTime : 0f;

        bool watchedTooLong = watchedTimer >= maxWatchTime;
        bool tooClose = shadow.DistanceToPlayer() <= shadow.WatcherMinDistance;
        bool timeUp = lifeTimer >= lifeTime;

        if (watchedTooLong || tooClose || timeUp)
            shadow.ReturnToPool();
    }

    public void Exit() { }
}