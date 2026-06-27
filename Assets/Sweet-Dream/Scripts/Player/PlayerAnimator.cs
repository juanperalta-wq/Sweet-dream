using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    private string currentAnim;

    private Vector2 moveInput;
    private bool isSprinting;

    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int RunHash = Animator.StringToHash("Run");

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        PlayerInputs.OnMoveInputChange += HandleMove;
        PlayerInputs.OnSprint += HandleSprint;
    }

    private void OnDisable()
    {
        PlayerInputs.OnMoveInputChange -= HandleMove;
        PlayerInputs.OnSprint -= HandleSprint;
    }

    private void HandleMove(Vector2 input) => moveInput = input;
    private void HandleSprint(bool sprinting) => isSprinting = sprinting;

    private void Update()
    {
        Animations();
    }

    #region Animations
    private void Animations()
    {
        if (moveInput == Vector2.zero)
            SetAnimation("Idle", IdleHash);
        else if (isSprinting)
            SetAnimation("Run", RunHash);
        else
            SetAnimation("Walk", WalkHash);
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