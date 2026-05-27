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

    [TabGroup("Referencias"), Required]
    [SerializeField] private Animator anim;

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
    [SerializeField] private string currentAnim;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private float currentSpeed;
    #endregion

    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int RunHash = Animator.StringToHash("Run");

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        agent.speed = walkSpeed;
    }

         void Update()
    {
        Detection();
        Animations();
        DetectPoint();
    }

    #region Animations
    public void Animations()
    {
        currentSpeed = agent.velocity.magnitude;

        if (currentSpeed < 0.1f)
            SetAnimation("Idle", IdleHash);
        else if (currentSpeed >= runSpeed * 0.5f)
            SetAnimation("Run", RunHash);
        else
            SetAnimation("Walk", WalkHash);
    }
    #endregion

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
            agent.ResetPath();
        }
    }
    #endregion

    #region SetAnimation
    public void SetAnimation(string state, int hash)
    {
        if (currentAnim == state) return;
        currentAnim = state;

        anim.ResetTrigger(IdleHash);
        anim.ResetTrigger(WalkHash);
        anim.ResetTrigger(RunHash);
        anim.SetTrigger(hash);
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
}