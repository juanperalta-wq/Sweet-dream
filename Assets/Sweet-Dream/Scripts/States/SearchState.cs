using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DulceSueño.Algorithms;

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
    private float searchMoveRadius = 8f;

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

        enemyController.Agent.speed = enemyController.WalkSpeed * 1.3f;
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
        if (enemyController.CanSeePlayer())
        {
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            Debug.Log("Jugador no encontrado, volviendo a patrullar.");
            stateMachine.ChangeState(enemyController.RoamState);
            return;
        }
        if (!enemyController.HasReachedDestination())
        {
            CheckIfStuck();
            return;
        }
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
                    Quaternion target = baseRotation * Quaternion.Euler(0f, lookAngles[lookStepIndex], 0f);

                    enemyController.transform.rotation = Quaternion.Slerp(enemyController.transform.rotation, target, 8f * Time.deltaTime);

                    lookStepIndex++;
                    lookTimer = 0f;
                }
                else
                {
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
        List<ZoneNode> candidates = new List<ZoneNode>();

        // O(z * n): recorre todas las zonas de patrullaje y todos los nodos de cada una
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
            Debug.Log("Search: sin nodos cercanos, esperando...");
            return;
        }

        // Insertion Sort: ordenamos los candidatos por distancia ascendente.
        // Antes se elegía un nodo al azar entre los candidatos; ahora el enemigo investiga
        // primero el punto más cercano, lo que es un comportamiento de búsqueda más creíble
        // (y más fácil de defender en la exposición que "Random.Range").
        //-> O(n^2) peor caso, O(n) si la lista ya viene casi ordenada (frecuente aquí,
        //   porque el enemigo se mueve poco entre una llamada y la siguiente)
        SortAlgorithms.InsertionSort(candidates, (a, b) =>
        {
            float distA = Vector3.Distance(enemyController.transform.position, a.transform.position);
            float distB = Vector3.Distance(enemyController.transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });

        ZoneNode target = candidates[0];
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