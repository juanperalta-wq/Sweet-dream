using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class BarneyController : MonoBehaviour
{
    #region Variables
    [TabGroup("Referencias"), Required]
    [SerializeField] private Transform player;

    [TabGroup("Referencias"), Required]
    [SerializeField] private NavMeshAgent agent;

    [TabGroup("Movimiento"), LabelWidth(110)]
    [Range(1, 10)]
    [SerializeField] private float walkSpeed = 1f;

    [TabGroup("Movimiento"), LabelWidth(110)]
    [Range(1, 10)]
    [SerializeField] private float runSpeed = 4f;

    [TabGroup("Detección"), LabelWidth(110)]
    [Range(1, 10)]
    [SerializeField] private float rangeVision = 10f;

    [TabGroup("Temporal"), Required]
    [SerializeField] private Transform pointA;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private bool playerDetected;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private float currentSpeed;
    #endregion

    public static readonly int IdleHash = Animator.StringToHash("Idle");
    public static readonly int WalkHash = Animator.StringToHash("Walk");
    public static readonly int RunHash = Animator.StringToHash("Run");

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
    }

    // Agrega esto en Update:
    void Update()
    {
        currentSpeed = agent.velocity.magnitude;
        Detection();

        if (!playerDetected)
            DetectPoint();
    }

    #region Detection
    public void Detection()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= rangeVision)
        {
            playerDetected = true;
            agent.speed = runSpeed;
            agent.ResetPath();
            agent.SetDestination(player.position);
        }
        else
        {
            playerDetected = false;
            agent.speed = walkSpeed;
        }
    }
    #endregion
    #region Temporal
    public void DetectPoint()
    {
        if (pointA == null) return;
        float distanceToPoint = Vector3.Distance(transform.position, pointA.position);
        if (distanceToPoint <= rangeVision)
            agent.SetDestination(pointA.position);
    }
    #endregion

    #region Getters Setters
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float RangeVision => rangeVision;
    public bool PlayerDetected => playerDetected;
    public float CurrentSpeed => currentSpeed;
    #endregion
}