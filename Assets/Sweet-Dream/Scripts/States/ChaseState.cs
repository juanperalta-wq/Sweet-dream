using UnityEngine;

public class ChaseState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;

    public ChaseState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("Enemigo empezo a perseguir al player");
    }
    public void Update()
    {
        float distanceToPlayer = Vector3.Distance(enemyController.transform.position, enemyController.PlayerTransform.position);
        if (distanceToPlayer <= enemyController.AttackRange)
        {
            stateMachine.ChangeState(enemyController.AttackState);
            return;
        }
        if (distanceToPlayer > enemyController.DetectionRange)
        {
            stateMachine.ChangeState(enemyController.RoamState);
            return;
        }

        enemyController.Agent.SetDestination(enemyController.PlayerTransform.position);
    }

    public void Exit()
    {
        Debug.Log("Saliendo del chase State");
        enemyController.Agent.ResetPath();
    }
}