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
        Debug.Log("Iniciando patrulla");

        enemyController.Agent.speed = enemyController.WalkSpeed;

        currentNode = enemyController.StartingNode;

        if (currentNode == null)
        {
            Debug.LogError("RoamState: no hay StartingNode asignado en EnemyController.");
            return;
        }
        currentZone = currentNode.Zone;
        visitedNodes = 0;

        targetNodesToVisit = Random.Range(enemyController.MinNodesPerZone, enemyController.MaxNodesPerZone + 1);

        enemyController.Agent.SetDestination(currentNode.transform.position);
        waitTimer = enemyController.NodeWaitTime;
    }

    public void Update()
    {
        if (enemyController.CanSeePlayer())
        {
            Debug.Log("Jugador detectado durante patrulla");
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }
        if (!enemyController.HasReachedDestination())
            return;

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            MoveToNextNode();
            waitTimer = enemyController.NodeWaitTime;
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
            ChangeZone();
            return;
        }

        if (currentNode == null || currentNode.Neighbors.Count == 0)
            return;

        ZoneNode nextNode;

        if (currentNode.Neighbors.Count == 1)
        {
            nextNode = currentNode.Neighbors[0];
        }
        else
        {
            // Evitar volver inmediatamente al nodo anterior
            int attempts = 0;
            do
            {
                nextNode = currentNode.Neighbors[Random.Range(0, currentNode.Neighbors.Count)];
                attempts++;
            }
            while (nextNode == previousNode && attempts < 10);
        }

        if (!CanReachNode(nextNode))
        {
            Debug.Log("RoamState: nodo inaccesible — " + nextNode.name);
            return;
        }

        previousNode = currentNode;
        currentNode = nextNode;

        enemyController.Agent.SetDestination(currentNode.transform.position);
        Debug.Log("Moviéndose hacia: " + currentNode.name);
    }

    private void ChangeZone()
    {
        if (enemyController.PatrolZones.Length == 0)
            return;

        PatrolZone nextZone;
        int attempts = 0;

        do
        {
            nextZone = enemyController.PatrolZones[Random.Range(0, enemyController.PatrolZones.Length)];
            attempts++;
        }
        while (nextZone == currentZone && enemyController.PatrolZones.Length > 1 && attempts < 10);

        currentZone = nextZone;

        if (currentZone.Nodes.Count == 0)
            return;

        currentNode = currentZone.Nodes[Random.Range(0, currentZone.Nodes.Count)];
        previousNode = null;
        visitedNodes = 0;

        targetNodesToVisit = Random.Range(enemyController.MinNodesPerZone, enemyController.MaxNodesPerZone + 1);

        enemyController.Agent.SetDestination(currentNode.transform.position);
        Debug.Log("Cambiando a zona: " + currentZone.name);
    }

    private bool CanReachNode(ZoneNode node)
    {
        NavMeshPath path = new NavMeshPath();
        enemyController.Agent.CalculatePath(node.transform.position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
}