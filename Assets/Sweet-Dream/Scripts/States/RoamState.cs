using Unity.VisualScripting;
using UnityEngine;

public class RoamState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;

    private ZoneNode currentNode;
    private float waitTimer;

    public RoamState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("INICIANDO PATROLLING");

        currentNode = enemyController.StartingNode;

        if (currentNode != null)
        {
            enemyController.Agent.SetDestination(
                currentNode.transform.position
            );
        }

        waitTimer = enemyController.NodeWaitTime;
    }

    public void Update()
    {
        if (PlayerInDetectionRange())
        {
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }

        if (!enemyController.Agent.pathPending &&
           enemyController.Agent.remainingDistance <= enemyController.Agent.stoppingDistance)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                MoveToNextNode();
                waitTimer = enemyController.NodeWaitTime;
            }
        }
    }
    public void Exit()
    {
        enemyController.Agent.ResetPath();
    }

    private void MoveToNextNode()
    {
        if (currentNode == null)
            return;

        if (currentNode.Neighbors.Count == 0)
            return;

        int randomIndex =
            Random.Range(0, currentNode.Neighbors.Count);

        currentNode = currentNode.Neighbors[randomIndex];

        enemyController.Agent.SetDestination(currentNode.transform.position);

        Debug.Log("Moviendose a: " + currentNode.name);
    }

    private bool PlayerInDetectionRange()
    {
        Collider[] hits =Physics.OverlapSphere(enemyController.transform.position,enemyController.DetectionRange);

        foreach (Collider collider in hits)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}