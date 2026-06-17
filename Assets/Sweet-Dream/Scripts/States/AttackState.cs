using Unity.VisualScripting;
using UnityEngine;

public class AttackState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;


    private float cooldownTimer;
    public AttackState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("En rango de ataque");
        enemyController.Agent.isStopped = true;
        cooldownTimer = 0;
    }
    public void Update()
    {
        float distanceToPlayer = Vector3.Distance(enemyController.transform.position, enemyController.PlayerTransform.position);

        if (distanceToPlayer > enemyController.AttackRange)
        {
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0)
        {
            PerformAttack();
            cooldownTimer = enemyController.AttackCooldown;
        }


        Vector3 directionToPlayer = (enemyController.PlayerTransform.position - enemyController.transform.position).normalized;
        directionToPlayer.y = 0f;

        enemyController.transform.rotation = Quaternion.LookRotation(directionToPlayer);

    }
    public void Exit()
    {
        Debug.Log("saliendo del estado de ataque");
        enemyController.Agent.isStopped = false;
    }

    public void PerformAttack()
    {
        Debug.Log("Atacando .... !");
    }


}