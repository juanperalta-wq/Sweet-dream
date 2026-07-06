using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    #region Detection
    [BoxGroup("Detection")]
    [MinValue(1)]
    public float DetectionRange = 50f;

    [BoxGroup("Detection")]
    [Range(0, 360)]
    public float ViewAngle = 180f;

    [BoxGroup("Detection")]
    [MinValue(0.5f)]
    public float AttackRange = 2f;

    [BoxGroup("Detection")]
    [MinValue(0)]
    public float MemoryDuration = 5f;

    [BoxGroup("Detection")]
    [Tooltip("Capas que bloquean la visión (paredes, obstáculos). " + "NO incluir la capa del propio enemigo.")]
    public LayerMask ObstacleMask;

    [BoxGroup("Detection")]
    [Tooltip("Capa del jugador para el raycast.")]
    public LayerMask PlayerMask;
    #endregion

    #region Combat
    [BoxGroup("Combat")]
    [MinValue(0.1f)]
    public float AttackCooldown = 1.5f;

    [BoxGroup("Combat")]
    [MinValue(1)]
    public float AttackDamage = 10f;
    #endregion

    #region Movement
    [BoxGroup("Movement")]
    public float WalkSpeed = 2f;

    [BoxGroup("Movement")]
    public float RunSpeed = 5f;

    [BoxGroup("Movement")]
    [Tooltip("Velocidad de rotación en grados/segundo al girar hacia el jugador.")]
    public float RotationSpeed = 10f;
    #endregion

    #region Patrol
    [BoxGroup("Patrol")]
    [Required]
    public ZoneNode StartingNode;

    [BoxGroup("Patrol")]
    public float NodeWaitTime = 3f;

    [BoxGroup("Patrol")]
    [Required]
    public PatrolZone[] PatrolZones;

    [BoxGroup("Patrol")]
    public int MinNodesPerZone = 2;

    [BoxGroup("Patrol")]
    public int MaxNodesPerZone = 5;
    #endregion

    #region Health
    [BoxGroup("Health")]
    public float MaxHealth = 100f;

    [BoxGroup("Health")]
    [ReadOnly]
    public float CurrentHealth;
    #endregion

    #region References
    [BoxGroup("References")]
    [Required]
    public Transform PlayerTransform;

    [BoxGroup("References")]
    [Required]
    public Transform EyePoint;

    [BoxGroup("References")]
    [ReadOnly]
    public NavMeshAgent Agent;
    #endregion

    #region Memory
    [FoldoutGroup("Memory")]
    [ReadOnly]
    public Vector3 LastSeenPosition;

    [FoldoutGroup("Memory")]
    [ReadOnly]
    public float LastTimeSeenPlayer;

    // Dirección (normalizada, plano XZ) hacia la que se movía el jugador la
    // última vez que lo vimos. Sirve para que la búsqueda continúe "de frente"
    // en vez de solo mirar la posición puntual donde se perdió.
    [FoldoutGroup("Memory")]
    [ReadOnly]
    public Vector3 LastSeenDirection;

    private bool hasSeenPlayerBefore = false;
    #endregion

    #region State Machine
    public StateMachine StateMachine;

    public RoamState RoamState;
    public ChaseState ChaseState;
    public SearchState SearchState;
    public AttackState AttackState;
    public DeadState DeadState;
    #endregion

    // Puerta cercana detectada durante la navegación
    // Los estados pueden consultar esto para decidir si abrir una puerta.
    [HideInInspector]
    public IDoor NearbyDoor;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        if (Agent == null)
            Debug.LogError("EnemyController necesita un NavMeshAgent.");

        // Si no se asignó ninguna capa de obstáculos, usar Default como fallback
        if (ObstacleMask == 0)
            ObstacleMask = LayerMask.GetMask("Default");
    }
    private void Start()
    {
        CurrentHealth = MaxHealth;

        StateMachine = new StateMachine();

        RoamState = new RoamState(StateMachine, this);
        ChaseState = new ChaseState(StateMachine, this);
        SearchState = new SearchState(StateMachine, this);
        AttackState = new AttackState(StateMachine, this);
        DeadState = new DeadState(StateMachine, this);

        StateMachine.Initialize(RoamState);
    }

    private void Update()
    {
        StateMachine.Update();
    }
    public void MoveTo(Vector3 destination)
    {
        Agent.isStopped = false;
        Agent.SetDestination(destination);
    }

    public void StopMoving()
    {
        Agent.ResetPath();
        Agent.isStopped = true;
    }

    public bool HasReachedDestination()
    {
        return !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance;
    }
    public void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion desired = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, RotationSpeed * Time.deltaTime);
    }
    public float DistanceToPlayer()
    {
        if (PlayerTransform == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, PlayerTransform.position);
    }

    public bool CanSeePlayer()
    {
        if (PlayerTransform == null || EyePoint == null) return false;

        Vector3 origin = EyePoint.position;
        Vector3 target = PlayerTransform.position + Vector3.up * 1f;
        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        // Fuera de rango
        if (distance > DetectionRange)
        {
            Debug.DrawLine(origin, origin + direction * DetectionRange, Color.grey);
            return false;
        }

        // Fuera del ángulo de visión
        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > ViewAngle * 0.5f) return false;

        // Raycast SOLO contra obstáculos hasta la distancia exacta al jugador.
        // Si nada lo bloquea, visible. Ya no depende del tag ni de PlayerMask.
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ObstacleMask))
        {
            // Algo bloqueó el camino antes de llegar al jugador
            Debug.DrawLine(origin, hit.point, Color.red);
            return false;
        }

        // Camino libre, jugador detectado
        Vector3 newSeenPosition = PlayerTransform.position;

        if (hasSeenPlayerBefore)
        {
            Vector3 moveDelta = newSeenPosition - LastSeenPosition;
            moveDelta.y = 0f;

            // Solo actualizamos la dirección si el movimiento es significativo,
            // para evitar que pequeños jitters de posición generen direcciones falsas
            if (moveDelta.sqrMagnitude > 0.04f) // ~0.2m
                LastSeenDirection = moveDelta.normalized;
        }

        LastSeenPosition = newSeenPosition;
        LastTimeSeenPlayer = Time.time;
        hasSeenPlayerBefore = true;

        Debug.DrawLine(origin, target, Color.green);
        return true;
    }

    public bool RemembersPlayer()
    {
        return Time.time - LastTimeSeenPlayer <= MemoryDuration;
    }
    public void TryOpenNearbyDoor()
    {
        if (NearbyDoor != null)
            NearbyDoor.Open(transform);
    }
    public void TakeDamage(float damage)
    {
        if (StateMachine.CurrentState == DeadState) return; // ya muerto

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            StateMachine.ChangeState(DeadState);
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Rango de detección
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        // Ángulo de visión
        Vector3 left = Quaternion.Euler(0, -ViewAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, ViewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, left * DetectionRange);
        Gizmos.DrawRay(transform.position, right * DetectionRange);
    }
}