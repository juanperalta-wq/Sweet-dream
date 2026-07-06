using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

// Sombra pooleada: nunca se destruye. Cuando termina su comportamiento se desactiva y
// regresa al ShadowPool. Reutiliza el mismo patrón de StateMachine + IState que ya usa
// EnemyController para el enemigo principal, pero con estados propios de alucinación.
[RequireComponent(typeof(NavMeshAgent))]
public class ShadowAI : MonoBehaviour
{
    #region Config Watcher
    [TabGroup("Watcher"), LabelWidth(150)]
    [Tooltip("Cuánto tiempo sostenido puede el jugador mirarla antes de que desaparezca.")]
    [SerializeField] private float watcherMaxWatchTime = 3f;
    [TabGroup("Watcher"), LabelWidth(150)]
    [SerializeField] private float watcherLifeTime = 8f;
    [TabGroup("Watcher"), LabelWidth(150)]
    [SerializeField] private float watcherMinDistance = 2.5f;
    #endregion

    #region Config Stalker
    [TabGroup("Stalker"), LabelWidth(150)]
    [SerializeField] private float stalkerSpeed = 1.5f;
    [TabGroup("Stalker"), LabelWidth(150)]
    [SerializeField] private float stalkerFollowDistance = 4f;
    [TabGroup("Stalker"), LabelWidth(150)]
    [SerializeField] private float stalkerTooCloseDistance = 2f;
    #endregion

    #region Config Rusher
    [TabGroup("Rusher"), LabelWidth(150)]
    [SerializeField] private float rusherSpeed = 6f;
    [TabGroup("Rusher"), LabelWidth(150)]
    [SerializeField] private float rusherHitRange = 1.2f;
    [TabGroup("Rusher"), LabelWidth(150)]
    [SerializeField] private float rusherMaxChaseTime = 6f;
    #endregion

    #region Config Phantom
    [TabGroup("Phantom"), LabelWidth(150)]
    [SerializeField] private float phantomVisibleDuration = 2f;
    [TabGroup("Phantom"), LabelWidth(150)]
    [SerializeField] private int phantomMaxTeleports = 3;
    #endregion

    #region Config Escalación / Variedad
    [TabGroup("Escalación"), LabelWidth(150)]
    [Tooltip("Segundos sostenidos que el Stalker debe pasar demasiado cerca del jugador antes de decidir si escala a Rusher.")]
    [SerializeField] private float stalkerEscalateAfter = 2f;
    [TabGroup("Escalación"), LabelWidth(150), Range(0f, 1f)]
    [Tooltip("Probabilidad de que el Stalker escale a Rusher en vez de simplemente desvanecerse.")]
    [SerializeField] private float stalkerEscalateChance = 0.35f;

    [TabGroup("Escalación"), LabelWidth(150), Range(0f, 1f)]
    [Tooltip("Probabilidad de que, en su último teletransporte, el Phantom termine embistiendo en vez de desaparecer.")]
    [SerializeField] private float phantomEscalateChance = 0.3f;

    [TabGroup("Escalación"), LabelWidth(150)]
    [Tooltip("Rango de variación aleatoria (jitter) aplicado a los tiempos/distancias de cada sombra al aparecer, para que instancias del mismo tipo no se sientan idénticas.")]
    [SerializeField] private Vector2 jitterRange = new Vector2(0.85f, 1.15f);
    #endregion

    // Evita que una misma sombra escale más de una vez en su vida (Stalker->Rusher o Phantom->Rusher).
    public bool HasEscalated { get; set; }

    // Multiplicador random generado en cada OnSpawned(); los estados lo usan para variar
    // sus propios tiempos/distancias en vez de usar siempre el mismo valor fijo del Inspector.
    public float JitterMultiplier { get; private set; } = 1f;

    public float StalkerEscalateAfter => stalkerEscalateAfter;
    public float StalkerEscalateChance => stalkerEscalateChance;
    public float PhantomEscalateChance => phantomEscalateChance;

    // Usado por StalkerState/PhantomState antes de escalar a Rusher: ¿hay distancia
    // suficiente para que el jugador tenga margen de reacción una vez que empiece a
    // embestir? (mismo criterio de "tiempo de reacción" que usa TickStuckWatchdog).
    public bool IsSafeDistanceToEscalate()
    {
        float safeDistance = Mathf.Max(rusherSpeed * minPhaseReactionTime, 1f);
        return DistanceToPlayer() >= safeDistance;
    }

    #region Config Puertas / Atascos
    [TabGroup("Puertas"), LabelWidth(150)]
    [Tooltip("Segundos de espera cuando el NavMesh confirma camino parcial/inválido (ej. puerta cerrada). Señal confiable: puede ser corto.")]
    [SerializeField] private float doorPhaseDelay = 1f;
    [TabGroup("Puertas"), LabelWidth(150)]
    [Tooltip("Segundos de espera cuando solo se detecta velocidad cero sin señal de camino parcial (atasco físico genérico, menos confiable, más conservador).")]
    [SerializeField] private float stuckTimeout = 2.5f;
    [TabGroup("Puertas"), LabelWidth(150)]
    [SerializeField] private float phaseSampleRadius = 3f;
    [TabGroup("Puertas"), LabelWidth(150)]
    [Tooltip("Segundos mínimos de reacción que se le garantizan al jugador antes de que una sombra reubicada pueda alcanzarlo. Se convierte en distancia según la velocidad de ESA sombra: una lenta (Watcher/Stalker) necesita reaparecer cerca, una rápida (Rusher) necesita reaparecer bastante más lejos para dar el mismo margen.")]
    [SerializeField] private float minPhaseReactionTime = 1f;
    #endregion

    private float pathBlockedTimer;
    private float stuckTimer;

    // Expuesto para que los estados puedan evitar recalcular destino mientras está
    // confirmado bloqueado (si no, cada recálculo puede resolver a un punto reachable
    // distinto -a veces una esquina- y se ve como que "vaga" antes de reubicarse).
    public bool IsPathBlocked =>
        !Agent.pathPending &&
        (Agent.pathStatus == NavMeshPathStatus.PathPartial || Agent.pathStatus == NavMeshPathStatus.PathInvalid);

    #region Config Visión
    [TabGroup("Visión"), LabelWidth(150)]
    [SerializeField] private float seenMaxAngle = 45f;
    [TabGroup("Visión"), LabelWidth(150)]
    [SerializeField] private float seenMaxDistance = 15f;
    [TabGroup("Visión"), LabelWidth(150)]
    [SerializeField] private LayerMask obstacleMask;
    #endregion

    [TabGroup("Config"), Required]
    [SerializeField] private ShadowType type;

    public NavMeshAgent Agent { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public ShadowType Type => type;

    public StateMachine StateMachine { get; private set; }
    public WatcherState WatcherState { get; private set; }
    public StalkerState StalkerState { get; private set; }
    public RusherState RusherState { get; private set; }
    public PhantomState PhantomState { get; private set; }

    #region Getters usados por los estados
    public float WatcherMaxWatchTime => watcherMaxWatchTime;
    public float WatcherLifeTime => watcherLifeTime;
    public float WatcherMinDistance => watcherMinDistance;
    public float StalkerSpeed => stalkerSpeed;
    public float StalkerFollowDistance => stalkerFollowDistance;
    public float StalkerTooCloseDistance => stalkerTooCloseDistance;
    public float RusherSpeed => rusherSpeed;
    public float RusherHitRange => rusherHitRange;
    public float RusherMaxChaseTime => rusherMaxChaseTime;
    public float PhantomVisibleDuration => phantomVisibleDuration;
    public int PhantomMaxTeleports => phantomMaxTeleports;
    #endregion

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        StateMachine = new StateMachine();
        WatcherState = new WatcherState(StateMachine, this);
        StalkerState = new StalkerState(StateMachine, this);
        RusherState = new RusherState(StateMachine, this);
        PhantomState = new PhantomState(StateMachine, this);
    }

    private void Update()
    {
        StateMachine?.Update();
    }

    // Llamado por ShadowPool.Get() cada vez que esta instancia se reutiliza desde el pool.
    public void OnSpawned(Vector3 position, Quaternion rotation)
    {
        if (PlayerStats.Instance != null)
            PlayerTransform = PlayerStats.Instance.transform;

        // Agent.Warp en vez de transform.position: evita el error típico de reposicionar
        // un NavMeshAgent que estuvo desactivado (pooling con NavMesh).
        Agent.Warp(position);
        transform.rotation = rotation;
        Agent.isStopped = false;

        HasEscalated = false;
        JitterMultiplier = UnityEngine.Random.Range(jitterRange.x, jitterRange.y);

        IState startState = type switch
        {
            ShadowType.Watcher => WatcherState,
            ShadowType.Stalker => StalkerState,
            ShadowType.Rusher => RusherState,
            ShadowType.Phantom => PhantomState,
            _ => WatcherState
        };

        StateMachine.Initialize(startState);
    }

    // Cada estado llama esto cuando termina su comportamiento: la sombra vuelve al pool
    // en vez de destruirse.
    public void ReturnToPool()
    {
        Agent.ResetPath();
        Agent.isStopped = true;

        if (ShadowManager.Instance != null)
            ShadowManager.Instance.NotifyReturned(this);

        if (ShadowPool.Instance != null)
            ShadowPool.Instance.Return(this);
    }

    // Usado por WatcherState y StalkerState: ¿el jugador está mirando directamente hacia
    // esta sombra en este momento? Mismo criterio que EnemyController.CanSeePlayer(),
    // pero invertido (aquí es el jugador el que "detecta" a la sombra).
    public bool IsSeenByPlayer()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 origin = cam.transform.position;
        Vector3 toShadow = transform.position - origin;
        float distance = toShadow.magnitude;

        if (distance > seenMaxDistance) return false;

        float angle = Vector3.Angle(cam.transform.forward, toShadow);
        if (angle > seenMaxAngle) return false;

        // Si algo bloquea el camino antes de llegar a la sombra, no está siendo vista.
        if (Physics.Raycast(origin, toShadow.normalized, distance, obstacleMask))
            return false;

        return true;
    }

    public float DistanceToPlayer()
    {
        if (PlayerTransform == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, PlayerTransform.position);
    }

    public void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        transform.rotation = Quaternion.LookRotation(direction);
    }
    public bool TickStuckWatchdog(Vector3 desiredTarget)
    {
        // Señal confiable: el NavMesh confirma que no puede completar el camino
        // (típico con una puerta cerrada). Reacciona rápido porque no es una suposición.
        pathBlockedTimer = IsPathBlocked ? pathBlockedTimer + Time.deltaTime : 0f;

        // Señal de respaldo: quedó con velocidad cero sin llegar a destino (atasco físico
        // genérico). Más lenta porque puede ser un falso positivo momentáneo.
        bool notMoving = !Agent.pathPending
            && Agent.remainingDistance > Agent.stoppingDistance + 0.1f
            && Agent.velocity.sqrMagnitude < 0.01f;
        stuckTimer = notMoving ? stuckTimer + Time.deltaTime : 0f;

        bool readyByPath = pathBlockedTimer >= doorPhaseDelay;
        bool readyByStuck = stuckTimer >= stuckTimeout;

        if (!readyByPath && !readyByStuck)
            return false;

        pathBlockedTimer = 0f;
        stuckTimer = 0f;
        Vector3 samplePoint = desiredTarget;

        if (PlayerTransform != null)
        {
            float minDistance = Mathf.Max(Agent.speed * minPhaseReactionTime, 1f);
            float distanceToPlayer = Vector3.Distance(desiredTarget, PlayerTransform.position);

            if (distanceToPlayer < minDistance)
            {
                Vector3 away = desiredTarget - PlayerTransform.position;
                if (away.sqrMagnitude < 0.0001f)
                    away = -transform.forward; // desiredTarget coincide justo con el jugador: usa cualquier dirección válida

                samplePoint = PlayerTransform.position + away.normalized * minDistance;
            }
        }

        if (NavMesh.SamplePosition(samplePoint, out NavMeshHit hit, phaseSampleRadius, NavMesh.AllAreas))
        {
            Agent.Warp(hit.position);
            return true;
        }

        return false;
    }
}