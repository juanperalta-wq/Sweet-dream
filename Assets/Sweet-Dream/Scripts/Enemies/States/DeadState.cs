using UnityEngine;

public class DeadState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;

    private float destroyDelay = 3f;

    public DeadState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("Barney murió");

        // Detener movimiento
        enemyController.Agent.ResetPath();
        enemyController.Agent.enabled = false;

        // Desactivar colisión
        Collider col = enemyController.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Destruir el objeto después del delay
        Object.Destroy(enemyController.gameObject, destroyDelay);
    }
    public void Update() { }
    public void Exit() { }
}