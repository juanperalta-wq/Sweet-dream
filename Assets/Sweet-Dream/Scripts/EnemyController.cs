using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Detection")]
    public float DetectionRange = 10f;
    public float AttackRange = 2f;

    [Header("Combat")]
    public float AttackCooldown = 1.5f;
    public float AttackDamage = 10f;

    [Header("Movement")]
    public float WalkSpeed = 1.5f;
    public float RunSpeed = 4f;

    [Header("Graph Patrol")]
    public ZoneNode StartingNode;
    public float NodeWaitTime = 3f;

    [Header("Zone Patrol")]
    public PatrolZone[] PatrolZones;
    public int MinNodesPerZone = 2;
    public int MaxNodesPerZone = 5;

    [Header("Health")]
    public float MaxHealth = 100f;
    public float CurrentHealth;

    [Header("Target")]
    public Transform PlayerTransform;

    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public StateMachine stateMachine;

    [HideInInspector] public RoamState RoamState;
    [HideInInspector] public ChaseState ChaseState;
    [HideInInspector] public AttackState AttackState;
    [HideInInspector] public DeadState DeadState;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;

        stateMachine = new StateMachine();

        RoamState = new RoamState(stateMachine, this);
        ChaseState = new ChaseState(stateMachine, this);
        AttackState = new AttackState(stateMachine, this);
        DeadState = new DeadState(stateMachine, this);

        stateMachine.Initialize(RoamState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            stateMachine.ChangeState(DeadState);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}