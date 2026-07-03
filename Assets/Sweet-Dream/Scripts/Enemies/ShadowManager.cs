using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    [Header("Shadow")]
    [SerializeField] private GameObject shadowPrefab;

    [SerializeField] private float spawnDistance = 10f;

    private bool spawned60;
    private bool spawned30;
    private bool spawned10;

    private void Update()
    {
        float sanity = PlayerStats.Instance.Sanity;

        if (sanity <= 60 && !spawned60)
        {
            SpawnShadow();
            spawned60 = true;
        }

        if (sanity <= 30 && !spawned30)
        {
            SpawnShadow();
            SpawnShadow();
            spawned30 = true;
        }

        if (sanity <= 10 && !spawned10)
        {
            SpawnShadow();
            SpawnShadow();
            SpawnShadow();
            spawned10 = true;
        }
    }

    private void SpawnShadow()
    {
        Transform player = PlayerStats.Instance.transform;

        Vector3 spawnPosition = player.position - player.forward * spawnDistance;
        spawnPosition += Random.insideUnitSphere * 2f;
        spawnPosition.y = player.position.y;

        Instantiate(shadowPrefab, spawnPosition, Quaternion.identity);
    }
}