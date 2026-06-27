using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class BarneyController : MonoBehaviour
{
    #region Variables
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

    public void Start()
    {
        agent.speed = walkSpeed;
    }

    void Update()
    {
        currentSpeed = agent.velocity.magnitude;
        
            DetectPoint();
    }
    #region Temporal
    public void DetectPoint()
    {
        if (pointA == null) return;
        float distanceToPoint = Vector3.Distance(transform.position, pointA.position);
        if (distanceToPoint <= rangeVision)
            agent.SetDestination(pointA.position);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BoxTransition"))
        {
            Destroy(gameObject);
            
        }

    }

    #region Getters Setters
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float RangeVision => rangeVision;
    public bool PlayerDetected => playerDetected;
    public float CurrentSpeed => currentSpeed;
    #endregion
}