using UnityEngine;

public class ChaseState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;

    private const float DestinationUpdateThreshold = 0.5f;

    // Cuántos segundos persiste persiguiendo sin ver al jugador antes de pasar a SearchState
    private float lostSightTimer = 0f;
    private float lostSightDuration = 3f; // segundos de "fe ciega"

    // Anti-stuck para puertas
    private float stuckTimer;
    private Vector3 lastPosition;
    private const float StuckInterval = 1.2f;
    private const float StuckMinMovement = 0.1f;

    private Vector3 lastKnownDestination;

    public ChaseState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("Persiguiendo al jugador");

        enemyController.SetAgentSpeed(enemyController.RunSpeed);
        enemyController.Agent.isStopped = false;

        enemyController.Agent.SetDestination(enemyController.PlayerTransform.position);

        lastKnownDestination = enemyController.PlayerTransform.position;
        lastPosition = enemyController.transform.position;
        lostSightTimer = 0f;
        stuckTimer = 0f;
    }

    public void Update()
    {
        float distance = enemyController.DistanceToPlayer();

        // Entrar en ataque 
        if (distance <= enemyController.AttackRange)
        {
            stateMachine.ChangeState(enemyController.AttackState);
            return;
        }

        if (enemyController.CanSeePlayer())
        {
            // Ve al jugador, resetear timer y actualizar destino
            lostSightTimer = 0f;

            float moved = Vector3.Distance(
                enemyController.PlayerTransform.position,
                lastKnownDestination);

            if (moved > DestinationUpdateThreshold)
            {
                lastKnownDestination = enemyController.PlayerTransform.position;
                enemyController.Agent.SetDestination(lastKnownDestination);
            }

            ResetStuckTimer();
        }
        else
        {
            // No ve al jugador, contar tiempo
            lostSightTimer += Time.deltaTime;

            // Seguir yendo a la última posición conocida
            enemyController.Agent.SetDestination(lastKnownDestination);

            CheckIfStuck();

            // Si lleva demasiado tiempo sin ver, SearchState
            if (lostSightTimer >= lostSightDuration)
            {
                Debug.Log("Perdí al jugador, iniciando búsqueda...");
                stateMachine.ChangeState(enemyController.SearchState);
            }
        }
    }

    public void Exit()
    {
        enemyController.Agent.ResetPath();
    }

    private void CheckIfStuck()
    {
        stuckTimer += Time.deltaTime;

        if (stuckTimer >= StuckInterval)
        {
            float distanceMoved = Vector3.Distance(enemyController.transform.position, lastPosition);

            if (distanceMoved < StuckMinMovement)
            {
                enemyController.TryOpenNearbyDoor();
                Debug.Log("Chase: atascado, intentando abrir puerta.");
            }

            lastPosition = enemyController.transform.position;
            stuckTimer = 0f;
        }
    }
    private void ResetStuckTimer()
    {
        stuckTimer = 0f;
        lastPosition = enemyController.transform.position;
    }
}