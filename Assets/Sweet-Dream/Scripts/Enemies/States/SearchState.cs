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

    // Rotacion al llegar a un punto
    private int lookStepIndex = 0;
    private float[] lookAngles = { 0f, 60f, -60f, 120f, -120f };
    private Quaternion baseRotation;
    private bool isLooking = false;

    // Suavizado de la rotacion
    private const float LookRotationSpeed = 4f;
    private const float LookAngleThreshold = 3f;
    private float lookInterval = 1f;
    private float lookHoldTimer = 0f;

    // Busqueda activa: el radio crece con el tiempo sin encontrar al jugador
    private const float MinSearchRadius = 5f;
    private const float MaxSearchRadius = 15f;

    // Cuanto premiamos a un nodo por continuar la direccion del jugador
    private const float DirectionAlignmentWeight = 6f;

    // Memoria de nodos ya revisados recientemente
    private Dictionary<ZoneNode, float> recentlySearched = new Dictionary<ZoneNode, float>();
    private const float NodeMemoryDuration = 30f;

    // Duda ocasional antes de moverse a un nuevo nodo
    private bool isHesitating = false;
    private float hesitationTimer = 0f;
    private const float HesitationCooldown = 4f;
    private float lastHesitationTime = -10f;

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

        enemyController.SetAgentSpeed(enemyController.WalkSpeed * 1.3f);
        enemyController.Agent.SetDestination(enemyController.LastSeenPosition);
        searchTimer = searchDuration;
        lookStepIndex = 0;
        lookInterval = 1f;
        lookHoldTimer = 0f;
        isLooking = false;
        isHesitating = false;
        hesitationTimer = 0f;

        recentlySearched.Clear();

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

        UpdateSearchIntensity();
        CleanExpiredNodeMemory();

        if (isHesitating)
        {
            hesitationTimer -= Time.deltaTime;
            if (hesitationTimer <= 0f)
            {
                isHesitating = false;
                MoveToNearbyNode();
            }
            return;
        }

        if (!enemyController.HasReachedDestination())
        {
            CheckIfStuck();
            return;
        }

        HandleLookingPhase();
    }

    public void Exit()
    {
        enemyController.Agent.ResetPath();
    }

    // Cuanto mas tiempo pasa sin encontrar al jugador, mas nervioso se mueve
    private void UpdateSearchIntensity()
    {
        float progress = 1f - (searchTimer / searchDuration);

        if (progress > 0.66f)
        {
            enemyController.SetAgentSpeed(enemyController.WalkSpeed * 1.6f);
            lookInterval = 0.6f;
        }
        else
        {
            enemyController.SetAgentSpeed(enemyController.WalkSpeed * 1.3f);
            lookInterval = 1f;
        }
    }

    private void HandleLookingPhase()
    {
        if (!isLooking)
        {
            isLooking = true;
            lookStepIndex = 0;
            lookHoldTimer = 0f;
            baseRotation = enemyController.transform.rotation;
        }

        Quaternion target = baseRotation * Quaternion.Euler(0f, lookAngles[lookStepIndex], 0f);

        enemyController.transform.rotation = Quaternion.Slerp(enemyController.transform.rotation, target, LookRotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(enemyController.transform.rotation, target) < LookAngleThreshold)
        {
            lookHoldTimer += Time.deltaTime;

            if (lookHoldTimer >= lookInterval)
            {
                lookHoldTimer = 0f;
                lookStepIndex++;

                if (lookStepIndex >= lookAngles.Length)
                {
                    isLooking = false;
                    MoveToNearbyNode();
                }
            }
        }
        else
        {
            lookHoldTimer = 0f;
        }
    }

    private void MoveToNearbyNode()
    {
        List<ZoneNode> candidates = CollectCandidateNodes();

        if (candidates.Count == 0)
        {
            Debug.Log("Search: sin nodos cercanos, esperando...");
            return;
        }

        SortAlgorithms.InsertionSort(candidates, (a, b) =>
        {
            float scoreA = ComputeNodeScore(a);
            float scoreB = ComputeNodeScore(b);
            return scoreA.CompareTo(scoreB);
        });

        ZoneNode target = PickWeightedNode(candidates);

        bool canHesitate = Time.time - lastHesitationTime >= HesitationCooldown;
        if (canHesitate && Random.value < 0.35f)
        {
            lastHesitationTime = Time.time;
            isHesitating = true;
            hesitationTimer = Random.Range(0.4f, 1.2f);
            Debug.Log("Search: dudando...");
            return;
        }

        recentlySearched[target] = Time.time;
        enemyController.Agent.SetDestination(target.transform.position);
        Debug.Log("Search: yendo a nodo - " + target.name);
    }

    private float ComputeNodeScore(ZoneNode node)
    {
        float distance = Vector3.Distance(enemyController.transform.position, node.transform.position);

        if (enemyController.LastSeenDirection != Vector3.zero)
        {
            Vector3 toNode = node.transform.position - enemyController.LastSeenPosition;
            toNode.y = 0f;

            if (toNode.sqrMagnitude > 0.01f)
            {
                float alignment = Vector3.Dot(toNode.normalized, enemyController.LastSeenDirection);
                distance -= alignment * DirectionAlignmentWeight;
            }
        }

        return distance;
    }

    private List<ZoneNode> CollectCandidateNodes()
    {
        List<ZoneNode> result = new List<ZoneNode>();

        float progress = 1f - (searchTimer / searchDuration);
        float radius = Mathf.Lerp(MinSearchRadius, MaxSearchRadius, progress);

        foreach (PatrolZone zone in enemyController.PatrolZones)
        {
            foreach (ZoneNode node in zone.Nodes)
            {
                if (recentlySearched.ContainsKey(node))
                    continue;

                float dist = Vector3.Distance(enemyController.transform.position, node.transform.position);

                if (dist <= radius && CanReachNode(node))
                    result.Add(node);
            }
        }

        return result;
    }

    private ZoneNode PickWeightedNode(List<ZoneNode> sortedCandidates)
    {
        int poolSize = Mathf.Min(3, sortedCandidates.Count);

        float[] weights = { 0.6f, 0.3f, 0.1f };
        float roll = Random.value;
        float acc = 0f;

        for (int i = 0; i < poolSize; i++)
        {
            acc += weights[i];
            if (roll <= acc)
                return sortedCandidates[i];
        }

        return sortedCandidates[0];
    }

    private void CleanExpiredNodeMemory()
    {
        List<ZoneNode> expired = null;

        foreach (KeyValuePair<ZoneNode, float> kv in recentlySearched)
        {
            if (Time.time - kv.Value > NodeMemoryDuration)
            {
                if (expired == null)
                    expired = new List<ZoneNode>();
                expired.Add(kv.Key);
            }
        }

        if (expired != null)
        {
            foreach (ZoneNode node in expired)
                recentlySearched.Remove(node);
        }
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