using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class BarneyController : MonoBehaviour
{
    #region Variables
    [FoldoutGroup("Variables")] 
    public Transform Player;
    [FoldoutGroup("Variables")] 
    public float WalkSpeed = 1f;
    [FoldoutGroup("Variables")] 
    public float RunSpeed = 4f;
    [FoldoutGroup("Variables")] 
    public float RangeVision = 10f;
    [FoldoutGroup("Variables")] 
    private NavMeshAgent agent;
    [FoldoutGroup("Variables")] 
    private Animator anim;
    [FoldoutGroup("Variables")]
    public bool PlayerDetected = false;
    [FoldoutGroup("Variables")]
    private string currentAnim = "";
    #endregion
    //temporal
    public Transform PointA;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        agent.speed = WalkSpeed;
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
        float currentSpeed = agent.velocity.magnitude;

        if (currentSpeed < 0.1f)
        {
            SetAnimation("Idle");
        }
        else if (currentSpeed >= RunSpeed * 0.5f)
        {
            SetAnimation("Run");
        }
        else
        {
            SetAnimation("Walk");
        }
    }
    #endregion
    #region Detection
    public void Detection()
    {
        if (Player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (distanceToPlayer <= RangeVision)
        {
            PlayerDetected = true;
            agent.speed = RunSpeed;
            agent.ResetPath();
            agent.SetDestination(Player.position);
        }
        else
        {
            PlayerDetected = false;
            agent.speed = WalkSpeed;
            agent.ResetPath();
        }
    }
    #endregion
    #region SetAnimation
    private void SetAnimation(string state)
    {
        if (currentAnim == state) return;
        currentAnim = state;

        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Walk");
        anim.ResetTrigger("Run");
        anim.SetTrigger(state);
    }
    #endregion
    //temporal
    public void DetectPoint()
    {
        float distanceToPoint = Vector3.Distance(transform.position, PointA.position);
        if(distanceToPoint <= RangeVision)
        {
            agent.SetDestination(PointA.position);
        }
    }
}