using UnityEngine;
 
public class EnemyAnimator : MonoBehaviour
{
    [SerializeField]
    private EnemyController controller;
 
    private Animator anim;
 
    // Hashes de los triggers del Animator (más rápido que strings)
    public static readonly int IdleHash = Animator.StringToHash("Idle");
    public static readonly int WalkHash = Animator.StringToHash("Walk");
    public static readonly int RunHash  = Animator.StringToHash("Run");
    //public static readonly int DeadHash = Animator.StringToHash("Dead");
 
    // Estado interno con enum para evitar comparaciones de strings
    private enum AnimState { None, Idle, Walk, Run, Dead }
    private AnimState currentAnimState = AnimState.None;
 
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
 
        if (anim == null)
            Debug.LogError("EnemyAnimator: no se encontró Animator en los hijos.");
 
        if (controller == null)
            controller = GetComponent<EnemyController>();
 
        if (controller == null)
            Debug.LogError("EnemyAnimator: no se encontró EnemyController.");
    }
 
    private void Update()
    {
        if (controller == null || controller.Agent == null || anim == null)
            return;
 
        // Si el enemigo está muerto, bloquear en animación de muerte
        /*if (controller.StateMachine?.CurrentState == controller.DeadState)
        {
            SetAnimation(AnimState.Dead, DeadHash);
            return;
        }*/
        float speed = controller.Agent.velocity.magnitude;
 
        // Umbral dinámico: caminando si < 60% de RunSpeed
        float runThreshold = controller.RunSpeed * 0.6f;
 
        if (speed < 0.1f)
            SetAnimation(AnimState.Idle, IdleHash);
        else if (speed >= runThreshold)
            SetAnimation(AnimState.Run, RunHash);
        else
            SetAnimation(AnimState.Walk, WalkHash);
    }
 
    private void SetAnimation(AnimState state, int hash)
    {
        if (currentAnimState == state) return;
 
        currentAnimState = state;
 
        // Limpiar todos los triggers antes de activar el nuevo
        anim.ResetTrigger(IdleHash);
        anim.ResetTrigger(WalkHash);
        anim.ResetTrigger(RunHash);
        //anim.ResetTrigger(DeadHash);
 
        anim.SetTrigger(hash);
    }
}