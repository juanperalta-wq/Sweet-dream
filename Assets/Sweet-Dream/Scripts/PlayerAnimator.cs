using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [FoldoutGroup("Referencias"), Required]
    [SerializeField] private FirstPersonController controller;

    private Animator anim;
    private string currentAnim;

    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int RunHash = Animator.StringToHash("Run");

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Animations();
    }

    #region Animations
    private void Animations()
    {
        float speed = controller.CurrentSpeed;

        if (speed < 0.1f)
        {
            SetAnimation("Idle", IdleHash);
        }
        else if (speed < controller.MoveSpeed)
        {
            SetAnimation("Walk", WalkHash);
        }
       /* else
        {
            SetAnimation("Run", RunHash);
        }*/
    }
    #endregion

    #region SetAnimation
    private void SetAnimation(string state, int hash)
    {
        if (currentAnim == state) return;
        currentAnim = state;

        anim.ResetTrigger(IdleHash);
        anim.ResetTrigger(WalkHash);
        anim.ResetTrigger(RunHash);
        anim.SetTrigger(hash);
    }
    #endregion
}