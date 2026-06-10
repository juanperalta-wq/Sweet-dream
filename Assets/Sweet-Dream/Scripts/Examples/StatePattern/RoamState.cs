using UnityEngine;
using UnityEngine.AI;

public class RoamState : IState
{
    private StateMachine stateMachine;
    private EnemyController enemyController;


    private float waitTimer;//->


    public RoamState(StateMachine stateMachine, EnemyController enemyController)
    {
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
    }

    public void Enter()
    {
        Debug.Log("INICIANDO ROAMING");
        waitTimer = 0;
    }
    public void Update()
    {
        if (PlayerInDetectionRange())
        {
            stateMachine.ChangeState(enemyController.ChaseState);
            return;
        }


        if (enemyController.Agent.remainingDistance <= enemyController.Agent.stoppingDistance)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer < 0)
            {
                SetRandomDestination();
                waitTimer = enemyController.RoamWaitTime;
            }
        }
    }

    public void Exit()
    {
        enemyController.Agent.ResetPath();
    }


    public void SetRandomDestination()
    {
        //-> vector3 randomdir = new vector3(randoimasdmamsdmasdmamsd).normalized
        Vector3 randomDirection = Random.insideUnitSphere * enemyController.RoamRadius;


        //randomDirection += enemyController.PlayerTransform.transform.position;
        randomDirection += enemyController.transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, enemyController.RoamRadius, NavMesh.AllAreas))
        {
            enemyController.Agent.SetDestination(hit.position);
            Debug.Log("Nuevo roam destiny");
        }
    }
    public bool PlayerInDetectionRange()
    {
        Collider[] hits = Physics.OverlapSphere(enemyController.transform.position, enemyController.DetectionRange);

        foreach (Collider collider in hits)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 dir = (enemyController.PlayerTransform.transform.position - enemyController.transform.position).normalized;

                if (Vector3.Dot(enemyController.transform.forward, dir) > 0.180f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;

    }
}

