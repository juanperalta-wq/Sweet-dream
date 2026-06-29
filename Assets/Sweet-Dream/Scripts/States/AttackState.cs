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
        Debug.Log("Entrando en estado de ataque");

        enemyController.Agent.isStopped = true;

        // Empezar con cooldown completo: el primer ataque requiere que el enemigo "se prepare" antes de golpear
        cooldownTimer = enemyController.AttackCooldown;
    }

    public void Update()
    {
        float distanceToPlayer = enemyController.DistanceToPlayer();

        // Si el jugador se aleja, volvemos a perseguir
        if (distanceToPlayer > enemyController.AttackRange)
        {
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }
        if (enemyController.PlayerTransform != null)
            enemyController.RotateTowards(enemyController.PlayerTransform.position);

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = enemyController.AttackCooldown;
        }
    }

    public void Exit()
    {
        enemyController.Agent.isStopped = false;
    }

    private void PerformAttack()
    {
        Debug.Log("Barney atacó al jugador por " + enemyController.AttackDamage + " de daño");

        // TODO: conectar con el sistema de salud del jugador:
        // PlayerHealth playerHealth = enemyController.PlayerTransform.GetComponent<PlayerHealth>();
        // if (playerHealth != null)
        //     playerHealth.TakeDamage(enemyController.AttackDamage);
    }
}