using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// Configuración de un tramo de cordura: cuántas sombras puede haber activas a la vez,
// con qué frecuencia aparecen y qué tipos están permitidos en ese tramo. Reemplaza los
// tres "if (sanity <= X && !spawnedX)" fijos del ShadowManager anterior por un sistema
// progresivo: mientras más baja la cordura, más tramos "agresivos" se activan.
[Serializable]
public class SanityStage
{
    [BoxGroup, MinMaxSlider(0, 100, true)]
    public Vector2 SanityRange = new Vector2(0, 100);

    [BoxGroup] public float MinSpawnInterval = 10f;
    [BoxGroup] public float MaxSpawnInterval = 18f;
    [BoxGroup] public int MaxActiveShadows = 1;
    [BoxGroup] public ShadowType[] AllowedTypes;
}

public class ShadowManager : MonoBehaviour
{
    public static ShadowManager Instance { get; private set; }

    [TabGroup("Stages"), Tooltip("Ordena los tramos de mayor a menor cordura. Ej: 100-60 (nada), 60-30 (Watcher), 30-10 (Watcher+Stalker), 10-0 (todo, incluyendo Rusher).")]
    [SerializeField] private SanityStage[] stages;

    [TabGroup("Spawn"), Tooltip("Puntos posibles para que aparezca una sombra: esquinas, pasillos, zonas fuera del campo de visión típico del jugador.")]
    [SerializeField] private Transform[] hiddenSpawnPoints;

    [TabGroup("Spawn"), LabelWidth(160)]
    [SerializeField] private float minDistanceFromPlayer = 4f;
    [TabGroup("Spawn"), LabelWidth(160)]
    [SerializeField] private float maxDistanceFromPlayer = 14f;
    [TabGroup("Spawn"), LabelWidth(160)]
    [SerializeField] private float outOfViewAngle = 60f;
    [TabGroup("Spawn"), LabelWidth(160)]
    [SerializeField] private LayerMask obstacleMask;

    [TabGroup("Spawn"), LabelWidth(160)]
    [Tooltip("Tiempo mínimo entre escalaciones a Rusher (Stalker->Rusher o Phantom->Rusher). Evita que, con sanidad baja y varias sombras activas, muchas escalen juntas y te caigan varios Rushers al mismo tiempo.")]
    [SerializeField] private float minTimeBetweenEscalations = 4f;

    private float lastEscalationTime = -999f;

    private readonly List<ShadowAI> activeShadows = new();
    private Coroutine spawnRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }
    private void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }
    public void NotifyReturned(ShadowAI shadow)
    {
        activeShadows.Remove(shadow);
    }
    public bool TryConsumeEscalationSlot()
    {
        if (Time.time - lastEscalationTime < minTimeBetweenEscalations)
            return false;

        lastEscalationTime = Time.time;
        return true;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SanityStage stage = GetCurrentStage();

            if (stage == null || stage.AllowedTypes == null || stage.AllowedTypes.Length == 0)
            {
                Debug.Log($"[ShadowManager] Sin tramo válido. Sanity actual: {(PlayerStats.Instance != null ? PlayerStats.Instance.Sanity.ToString() : "sin PlayerStats.Instance")}");
                // Sanidad alta / ningún tramo aplica: todavía no debe aparecer nada.
                yield return null;
                continue;
            }

            activeShadows.RemoveAll(s => s == null || !s.gameObject.activeInHierarchy);

            if (activeShadows.Count < stage.MaxActiveShadows)
                TrySpawnShadow(stage);
            else
                Debug.Log($"[ShadowManager] No spawneo: ya hay {activeShadows.Count}/{stage.MaxActiveShadows} activas.");

            float wait = UnityEngine.Random.Range(stage.MinSpawnInterval, stage.MaxSpawnInterval);
            yield return new WaitForSeconds(wait);
        }
    }
    //-> O(n) sobre la cantidad de tramos configurados (normalmente 3-5, costo mínimo)
    private SanityStage GetCurrentStage()
    {
        if (PlayerStats.Instance == null) return null;
        float sanity = PlayerStats.Instance.Sanity;

        foreach (SanityStage stage in stages)
        {
            float min = Mathf.Min(stage.SanityRange.x, stage.SanityRange.y);
            float max = Mathf.Max(stage.SanityRange.x, stage.SanityRange.y);

            if (sanity >= min && sanity <= max)
                return stage;
        }
        return null;
    }
    private void TrySpawnShadow(SanityStage stage)
    {
        Transform point = GetHiddenSpawnPoint(Vector3.zero);
        if (point == null)
        {
            Debug.Log("[ShadowManager] No spawneo: GetHiddenSpawnPoint() no encontró ningún punto válido (revisa el log de arriba de IsOutOfPlayerView/distancias).");
            return;
        }
        ShadowType type = stage.AllowedTypes[UnityEngine.Random.Range(0, stage.AllowedTypes.Length)];

        ShadowAI shadow = ShadowPool.Instance.Get(type, point.position, point.rotation);
        if (shadow != null)
        {
            activeShadows.Add(shadow);
            Debug.Log($"[ShadowManager] Spawneada sombra tipo {type} en {point.name}.");
        }
        else
        {
            Debug.LogWarning($"[ShadowManager] ShadowPool.Get() devolvió null para el tipo {type}. Revisa que ese tipo esté en poolEntries con un prefab asignado.");
        }
    }
    // Usado también por PhantomState para reubicarse en otro punto oculto sin pasar por el pool.
    public Transform RequestReposition(Vector3 excludeNear)
    {
        return GetHiddenSpawnPoint(excludeNear);
    }
    private Transform GetHiddenSpawnPoint(Vector3 excludeNear)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[ShadowManager] Camera.main es NULL. ¿La cámara del jugador tiene el tag 'MainCamera'?");
            return null;
        }
        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("[ShadowManager] PlayerStats.Instance es NULL.");
            return null;
        }
        if (hiddenSpawnPoints == null || hiddenSpawnPoints.Length == 0)
        {
            Debug.LogWarning("[ShadowManager] hiddenSpawnPoints está vacío. Arrastra los SpawnPoint en el Inspector.");
            return null;
        }
        Vector3 playerPos = PlayerStats.Instance.transform.position;
        List<Transform> candidates = new List<Transform>();

        foreach (Transform point in hiddenSpawnPoints)
        {
            if (point == null) continue;

            float distanceToPlayer = Vector3.Distance(point.position, playerPos);
            if (distanceToPlayer < minDistanceFromPlayer || distanceToPlayer > maxDistanceFromPlayer)
            {
                Debug.Log($"[ShadowManager] {point.name} descartado: distancia {distanceToPlayer:F1} fuera de [{minDistanceFromPlayer}, {maxDistanceFromPlayer}].");
                continue;
            }

            if (excludeNear != Vector3.zero && Vector3.Distance(point.position, excludeNear) < 1f)
                continue; // evita reubicarse prácticamente en el mismo lugar (PhantomState)

            if (!IsOutOfPlayerView(cam, point.position))
            {
                Debug.Log($"[ShadowManager] {point.name} descartado: está dentro del campo de visión del jugador.");
                continue;
            }

            candidates.Add(point);
        }
        if (candidates.Count == 0)
        {
            Debug.LogWarning("[ShadowManager] Ningún punto pasó los filtros. Revisa los logs de arriba.");
            return null;
        }

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }
    private bool IsOutOfPlayerView(Camera cam, Vector3 point)
    {
        Vector3 toPoint = point - cam.transform.position;
        float angle = Vector3.Angle(cam.transform.forward, toPoint);

        if (angle > outOfViewAngle)
            return true; // fuera del cono de visión: punto válido
        // Está dentro del cono, pero puede seguir siendo válido si algo lo bloquea (una pared, por ejemplo)
        return Physics.Raycast(cam.transform.position, toPoint.normalized, toPoint.magnitude, obstacleMask);
    }

    private void OnDrawGizmosSelected()
    {
        if (hiddenSpawnPoints == null) return;
        Gizmos.color = Color.magenta;
        foreach (Transform point in hiddenSpawnPoints)
        {
            if (point == null) continue;
            Gizmos.DrawWireSphere(point.position, 0.4f);
        }
    }
}