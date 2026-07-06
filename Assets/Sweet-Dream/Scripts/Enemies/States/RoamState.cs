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

    // Solo la primera vez que arranca el estado usamos StartingNode.
    // Las siguientes veces (por ejemplo, al volver de SearchState) retomamos
    // desde el nodo más cercano a la posición actual del enemigo.
    private bool hasStartedOnce = false;

    public RoamState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("Iniciando patrulla");

        enemyController.Agent.speed = enemyController.WalkSpeed;

        if (!hasStartedOnce)
        {
            currentNode = enemyController.StartingNode;
            hasStartedOnce = true;
        }
        else
        {
            currentNode = FindNearestReachableNode();
            Debug.Log($"[Roam] hasStartedOnce={hasStartedOnce} | posición enemigo={enemyController.transform.position} | nodo elegido={currentNode?.name} | pos nodo={currentNode?.transform.position}");
        }

        if (currentNode == null)
        {
            Debug.LogError("RoamState: no hay nodo válido para iniciar patrulla.");
            return;
        }

        currentZone = currentNode.Zone;
        previousNode = null;
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

    // Busca el nodo alcanzable más cercano a la posición actual del enemigo,
    // recorriendo todas las zonas de patrullaje. Se usa al retomar la patrulla
    // después de perseguir/buscar al jugador desde cualquier punto del mapa.
    private ZoneNode FindNearestReachableNode()
    {
        ZoneNode nearest = null;
        float nearestDist = Mathf.Infinity;

        foreach (PatrolZone zone in enemyController.PatrolZones)
        {
            foreach (ZoneNode node in zone.Nodes)
            {
                float dist = Vector3.Distance(enemyController.transform.position, node.transform.position);

                if (dist < nearestDist && CanReachNode(node))
                {
                    nearestDist = dist;
                    nearest = node;
                }
            }
        }

        // Fallback por si por algún motivo no se encontró ningún nodo alcanzable
        return nearest != null ? nearest : enemyController.StartingNode;
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