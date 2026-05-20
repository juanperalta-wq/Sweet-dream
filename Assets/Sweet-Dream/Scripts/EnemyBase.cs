using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [FoldoutGroup("Variables")]
    public bool diesFromCamera;
    public bool scaredOfFlashlight;

    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    public void OnPhotoHit()
    {
        Debug.Log(name + " recibió foto");

        if (diesFromCamera)
        {
            Die();
        }
        else
        {
            enemyAI.StopMovement(2f);
        }
    }

    public void OnFlashlightHit()
    {
        Debug.Log(name + " iluminado");

        if (scaredOfFlashlight)
        {
            enemyAI.StopMovement(1f);
        }
    }

    void Die()
    {
        Debug.Log(name + " murió");

        Destroy(gameObject);
    }
}