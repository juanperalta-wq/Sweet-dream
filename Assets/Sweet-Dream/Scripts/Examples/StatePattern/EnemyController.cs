using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Detection")]
    public float DetectionRange = 10f;
    public float AttackRange = 2f;
    [Header("Combat")]
    public float AttackCooldown = 1.5f;
    public float AttackDamage = 10;
    [Header("Roam")]
    public float RoamRadius = 15f;
    public float RoamWaitTime = 2f;
    [Header("HealthPoints")]
    public float MaxHealth = 100f;
    public float CurrentHealth;
    [Header("Target")]
    public Transform PlayerTransform;


    public NavMeshAgent Agent;
    public StateMachine stateMachine;

    public RoamState RoamState;
    public ChaseState ChaseState;
    public AttackState AttackState;
    public DeadState DeadState;
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        CurrentHealth = MaxHealth;
        stateMachine = new StateMachine();

        RoamState = new RoamState(stateMachine, this);
        ChaseState = new ChaseState(stateMachine, this);
        AttackState = new AttackState(stateMachine, this);
        DeadState = new DeadState(stateMachine, this);

        stateMachine.Initialize(RoamState);

    }
    void Update()
    {
        stateMachine.Update();
    }
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0)
        {
            stateMachine.ChangeState(DeadState);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, RoamRadius);
    }
}

