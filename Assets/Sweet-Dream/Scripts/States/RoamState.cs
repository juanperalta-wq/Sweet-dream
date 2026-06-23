using UnityEngine;
using UnityEngine.AI;

public class RoamState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;

    private ZoneNode currentNode;
    private ZoneNode previousNode;

    private PatrolZone currentZone;

    private float waitTimer;

    private int visitedNodes;
    private int targetNodesToVisit;

    public RoamState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("INICIANDO PATROLLING");

        currentNode = enemyController.StartingNode;

        if (currentNode == null)
            return;

        currentZone = currentNode.Zone;
        visitedNodes = 0;
        targetNodesToVisit = Random.Range(enemyController.MinNodesPerZone, enemyController.MaxNodesPerZone + 1);

        enemyController.Agent.SetDestination(currentNode.transform.position);

        waitTimer = enemyController.NodeWaitTime;
    }

    public void Update()
    {
        if (PlayerInDetectionRange())
        {
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }

        if (!enemyController.Agent.pathPending &&  enemyController.Agent.remainingDistance <= enemyController.Agent.stoppingDistance)
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
        visitedNodes++;

        if (visitedNodes >= targetNodesToVisit)
        {
            Debug.Log("Intentando cambiar de zona");
            ChangeZone();
            return;
        }

        if (currentNode == null)
            return;

        if (currentNode.Neighbors.Count == 0)
            return;

        ZoneNode nextNode;

        if (currentNode.Neighbors.Count == 1)
        {
            nextNode = currentNode.Neighbors[0];
        }
        else
        {
            do
            {
                nextNode = currentNode.Neighbors[Random.Range(0, currentNode.Neighbors.Count)];
            }
            while (nextNode == previousNode);
        }

        if (!CanReachNode(nextNode))
        {
            Debug.Log("Nodo inaccesible: " + nextNode.name);
            return;
        }

        previousNode = currentNode;
        currentNode = nextNode;

        enemyController.Agent.SetDestination(currentNode.transform.position);

        Debug.Log("Moving To: " + currentNode.name);
    }

    private void ChangeZone()
    {
        Debug.Log("Entró a ChangeZone()");
        if (enemyController.PatrolZones.Length == 0)
            return;

        PatrolZone nextZone;
        do
        {
            nextZone = enemyController.PatrolZones[Random.Range(0,enemyController.PatrolZones.Length)];
        }
        while (nextZone == currentZone && enemyController.PatrolZones.Length > 1);

        currentZone = nextZone;

        if (currentZone.Nodes.Count == 0)
            return;

        currentNode = currentZone.Nodes[Random.Range(0, currentZone.Nodes.Count)];
        previousNode = null;
        visitedNodes = 0;

        targetNodesToVisit = Random.Range(enemyController.MinNodesPerZone, enemyController.MaxNodesPerZone + 1);

        enemyController.Agent.SetDestination(currentNode.transform.position);

        Debug.Log("CAMBIANDO A ZONA: " + currentZone.name);
    }
    private bool CanReachNode(ZoneNode node)
    {
        NavMeshPath path = new NavMeshPath();

        enemyController.Agent.CalculatePath(node.transform.position,path);

        return path.status == NavMeshPathStatus.PathComplete;
    }

    private bool PlayerInDetectionRange()
    {
        Collider[] hits = Physics.OverlapSphere(enemyController.transform.position,enemyController.DetectionRange);

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