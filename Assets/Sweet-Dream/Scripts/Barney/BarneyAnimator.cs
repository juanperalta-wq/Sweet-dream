using UnityEngine;

public class BarneyAnimator : MonoBehaviour
{
    [SerializeField] private BarneyController controller;
    private Animator anim;

    private string currentAnim;

    public static readonly int IdleHash = Animator.StringToHash("Idle");
    public static readonly int WalkHash = Animator.StringToHash("Walk");
    public static readonly int RunHash = Animator.StringToHash("Run");

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Animations();
    }

    #region Animations
    public void Animations()
    {
        if (controller.CurrentSpeed < 0.1f)
            SetAnimation("Idle", IdleHash);
        else if (controller.CurrentSpeed >= controller.RunSpeed * 0.5f)
            SetAnimation("Run", WalkHash);
        else
            SetAnimation("Walk", RunHash);
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
}
