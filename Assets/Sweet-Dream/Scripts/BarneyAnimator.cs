using UnityEngine;

public class BarneyAnimator : MonoBehaviour
{
    [SerializeField] private BarneyController controller;
    private Animator anim;

    private string currentAnim;

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
            SetAnimation("Idle", BarneyController.IdleHash);
        else if (controller.CurrentSpeed >= controller.RunSpeed * 0.5f)
            SetAnimation("Run", BarneyController.RunHash);
        else
            SetAnimation("Walk", BarneyController.WalkHash);
    }
    #endregion

    #region SetAnimation
    public void SetAnimation(string state, int hash)
    {
        if (currentAnim == state) return;
        currentAnim = state;

        anim.ResetTrigger(BarneyController.IdleHash);
        anim.ResetTrigger(BarneyController.WalkHash);
        anim.ResetTrigger(BarneyController.RunHash);
        anim.SetTrigger(hash);
    }
    #endregion
}
