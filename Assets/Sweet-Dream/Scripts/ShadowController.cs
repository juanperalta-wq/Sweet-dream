using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private EnemyBaseData shadow;
    [SerializeField] private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void OnPhotoHit()
    {
        
            Destroy(gameObject); 
    }
}
