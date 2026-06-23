using UnityEngine;
using UnityEngine.AI;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    private NavMeshAgent agent;
    private Transform player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (PlayerStats.Instance != null)
        {
            player = PlayerStats.Instance.transform;
        }

        agent.speed = speed;
    }

    private void Update()
    {
        if (player == null)
            return;

        agent.SetDestination(player.position);
    }

    public void DestroyShadow()
    {
        Destroy(gameObject);
    }
}