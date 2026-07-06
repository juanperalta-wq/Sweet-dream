using System;
using System.Collections.Generic;
using UnityEngine;
using DulceSueño.Collections;
using Sirenix.OdinInspector;

[Serializable]
public class ShadowPoolEntry
{
    public ShadowType Type;
    public ShadowAI Prefab;
    [Min(0)] public int InitialSize = 5;
}
public class ShadowPool : MonoBehaviour
{
    public static ShadowPool Instance { get; private set; }

    [TabGroup("Config")]
    [SerializeField] private List<ShadowPoolEntry> poolEntries = new();
    private HashMap<ShadowType, DulceSueño.Collections.Queue<ShadowAI>> pools;

    // HashMap propio: qué prefab corresponde a cada tipo. Se construye una sola vez desde poolEntries.
    private HashMap<ShadowType, ShadowAI> prefabsByType;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BuildLookup();
    }
    private void Start()
    {
        foreach (ShadowPoolEntry entry in poolEntries)
            Prewarm(entry.Type, entry.InitialSize);
    }
    //-> O(n): recorre una sola vez la lista del Inspector y llena los HashMap de prefabs y colas.
    private void BuildLookup()
    {
        prefabsByType = new HashMap<ShadowType, ShadowAI>();
        pools = new HashMap<ShadowType, DulceSueño.Collections.Queue<ShadowAI>>();

        foreach (ShadowPoolEntry entry in poolEntries)
        {
            if (entry.Prefab == null)
            {
                Debug.LogWarning($"ShadowPool: entrada del tipo {entry.Type} no tiene prefab asignado.");
                continue;
            }

            prefabsByType.Add(entry.Type, entry.Prefab);
            pools.Add(entry.Type, new DulceSueño.Collections.Queue<ShadowAI>());
        }
    }
    private void Prewarm(ShadowType type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            ShadowAI instance = CreateNew(type);
            if (instance == null) continue;

            instance.gameObject.SetActive(false);

            if (pools.TryGetValue(type, out DulceSueño.Collections.Queue<ShadowAI> queue))
                queue.Enqueue(instance);
        }
    }
    private ShadowAI CreateNew(ShadowType type)
    {
        if (!prefabsByType.TryGetValue(type, out ShadowAI prefab) || prefab == null)
        {
            Debug.LogWarning($"ShadowPool: no hay prefab configurado para el tipo {type}.");
            return null;
        }

        return Instantiate(prefab, transform);
    }
    //-> O(1) promedio: un Dequeue de la cola propia + una búsqueda en el HashMap.
    public ShadowAI Get(ShadowType type, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(type, out DulceSueño.Collections.Queue<ShadowAI> queue))
        {
            Debug.LogWarning($"ShadowPool: tipo {type} no registrado en poolEntries.");
            return null;
        }

        ShadowAI shadow = queue.Count > 0 ? queue.Dequeue() : CreateNew(type);
        if (shadow == null) return null;

        shadow.gameObject.SetActive(true);
        shadow.OnSpawned(position, rotation);

        return shadow;
    }
    // La sombra vuelve aquí en vez de destruirse: se desactiva y queda lista para que
    // Get() la entregue de nuevo más adelante.
    public void Return(ShadowAI shadow)
    {
        if (shadow == null) return;

        shadow.gameObject.SetActive(false);

        if (pools.TryGetValue(shadow.Type, out DulceSueño.Collections.Queue<ShadowAI> queue))
            queue.Enqueue(shadow);
        else
            Debug.LogWarning($"ShadowPool: se intentó devolver una sombra de tipo no registrado ({shadow.Type}).");
    }
}