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
        enemyController.Agent.ResetPath();
        enemyController.Agent.enabled = false;

        Collider col = enemyController.GetComponent<Collider>();
        if (col != null) col.enabled = false;




        Object.Destroy(enemyController.gameObject, destroyDelay);

        Debug.Log("Destruyendose");
    }
    public void Update()
    {

    }

    public void Exit()
    {
        Debug.Log("Enemigo destruido pipipi");
    }
}