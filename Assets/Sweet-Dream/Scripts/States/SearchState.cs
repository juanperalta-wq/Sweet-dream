using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;

    // Tiempo total buscando antes de rendirse
    private float searchDuration = 20f;
    private float searchTimer;

    // Rotación al llegar a un punto
    private float lookTimer;
    private float lookInterval = 1f;
    private int lookStepIndex = 0;
    private float[] lookAngles = { 0f, 60f, -60f, 120f, -120f };
    private Quaternion baseRotation;
    private bool isLooking = false;

    // Búsqueda activa por nodos cercanos
    private float searchMoveRadius = 8f;  // radio para buscar nodos cercanos

    // Anti-stuck
    private float stuckTimer;
    private Vector3 lastPosition;
    private const float StuckInterval = 1.5f;
    private const float StuckMinMovement = 0.1f;

    public SearchState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("Buscando al jugador activamente...");

        enemyController.Agent.speed = enemyController.WalkSpeed * 1.3f; // un poco más rápido que pasear
        enemyController.Agent.SetDestination(enemyController.LastSeenPosition);
        searchTimer = searchDuration;
        lookTimer = 0f;
        lookStepIndex = 0;
        isLooking = false;

        lastPosition = enemyController.transform.position;
        stuckTimer = 0f;
    }

    public void Update()
    {
        // Si vuelve a ver al jugador
        if (enemyController.CanSeePlayer())
        {
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }
        searchTimer -= Time.deltaTime;
        // Se rindió
        if (searchTimer <= 0f)
        {
            Debug.Log("Jugador no encontrado, volviendo a patrullar.");
            stateMachine.ChangeState(enemyController.RoamState);
            return;
        }
        // Aún viajando hacia un punto
        if (!enemyController.HasReachedDestination())
        {
            CheckIfStuck();
            return;
        }
        // Llegó a un punto: mirar alrededor primero
        if (!isLooking)
        {
            isLooking = true;
            lookStepIndex = 0;
            lookTimer = 0f;
            baseRotation = enemyController.transform.rotation;
        }

        if (isLooking)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookInterval)
            {
                if (lookStepIndex < lookAngles.Length)
                {
                    // Rotar a siguiente ángulo
                    Quaternion target = baseRotation *Quaternion.Euler(0f, lookAngles[lookStepIndex], 0f);

                    enemyController.transform.rotation = Quaternion.Slerp(enemyController.transform.rotation, target, 8f * Time.deltaTime);

                    lookStepIndex++;
                    lookTimer = 0f;
                }
                else
                {
                    // Terminó de mirar, ir al siguiente nodo cercano
                    isLooking = false;
                    MoveToNearbyNode();
                }
            }
        }
    }
    public void Exit()
    {
        enemyController.Agent.ResetPath();
    }
    private void MoveToNearbyNode()
    {
        // Buscar todos los nodos dentro del radio de búsqueda
        List<ZoneNode> candidates = new List<ZoneNode>();

        // Buscar en todas las zonas de patrulla
        foreach (PatrolZone zone in enemyController.PatrolZones)
        {
            foreach (ZoneNode node in zone.Nodes)
            {
                float dist = Vector3.Distance(enemyController.transform.position, node.transform.position);

                if (dist <= searchMoveRadius && CanReachNode(node))
                    candidates.Add(node);
            }
        }

        if (candidates.Count == 0)
        {
            // No hay nodos cercanos, solo mirar y esperar
            Debug.Log("Search: sin nodos cercanos, esperando...");
            return;
        }

        // Elegir un nodo aleatorio de los candidatos
        ZoneNode target = candidates[Random.Range(0, candidates.Count)];
        enemyController.Agent.SetDestination(target.transform.position);
        Debug.Log("Search: moviéndose a nodo cercano — " + target.name);
    }

    private bool CanReachNode(ZoneNode node)
    {
        NavMeshPath path = new NavMeshPath();
        enemyController.Agent.CalculatePath(node.transform.position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void CheckIfStuck()
    {
        stuckTimer += Time.deltaTime;

        if (stuckTimer >= StuckInterval)
        {
            float moved = Vector3.Distance(enemyController.transform.position, lastPosition);
            if (moved < StuckMinMovement)
            {
                enemyController.TryOpenNearbyDoor();
                Debug.Log("Search: agente atascado, intentando abrir puerta.");
            }
            lastPosition = enemyController.transform.position;
            stuckTimer = 0f;
        }
    }
}