using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    #region variables
    [FoldoutGroup("Variables")]
    [SerializeField] private Transform player;
    [FoldoutGroup("Variables")]
    [SerializeField]private NavMeshAgent agent;
    [FoldoutGroup("Variables")]
    [SerializeField]private EnemyBase enemyBase;
    [FoldoutGroup("Variables")]
    [SerializeField]private bool canMove = true;
    #endregion
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyBase = GetComponent<EnemyBase>();
    }

    void Update()
    {
        if (canMove)
        {
            agent.SetDestination(player.position);
        }
    }

    public void StopMovement(float duration)
    {
        StartCoroutine(StopCoroutine(duration));
    }

    System.Collections.IEnumerator StopCoroutine(float duration)
    {
        canMove = false;

        agent.isStopped = true;

        yield return new WaitForSeconds(duration);

        agent.isStopped = false;

        canMove = true;
    }
}