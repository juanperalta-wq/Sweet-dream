using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private EnemyController controller;

    private Animator anim;
    private string currentAnim;

    public static readonly int IdleHash = Animator.StringToHash("Idle");
    public static readonly int WalkHash = Animator.StringToHash("Walk");
    public static readonly int RunHash = Animator.StringToHash("Run");

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();

        if (controller == null)
        {
            controller = GetComponent<EnemyController>();
        }
    }

    private void Update()
    {
        if (controller == null)
            return;

        if (controller.Agent == null)
            return;

        float speed = controller.Agent.velocity.magnitude;

        if (speed < 0.1f)
        {
            SetAnimation("Idle", IdleHash);
        }
        else if (speed >= 5f)
        {
            SetAnimation("Run", RunHash);
        }
        else
        {
            SetAnimation("Walk", WalkHash);
        }
    }

    private void SetAnimation(string state, int hash)
    {
        if (currentAnim == state)
            return;

        currentAnim = state;

        anim.ResetTrigger(IdleHash);
        anim.ResetTrigger(WalkHash);
        anim.ResetTrigger(RunHash);

        anim.SetTrigger(hash);
    }
}