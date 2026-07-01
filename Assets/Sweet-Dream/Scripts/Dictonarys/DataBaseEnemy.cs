using System.Collections.Generic;
using UnityEngine;
using DulceSueño.Collections;

[CreateAssetMenu(fileName = "DataBaseEnemy", menuName = "Scriptable Objects/DataBaseEnemy")]
public class DataBaseEnemy : ScriptableObject
{
    // Se mantiene TAL CUAL para no perder lo que ya llenaste a mano en el Inspector.
    public Dictionary<EnemyType, EnemyBaseData> dataBaseEnemies = new();

    // HashMap propio: la estructura que realmente usa el juego en tiempo de ejecución.
    private HashMap<EnemyType, EnemyBaseData> enemiesByType;

    //-> O(n): recorre una sola vez el Dictionary del Inspector y llena el HashMap.
    private void BuildLookup()
    {
        enemiesByType = new HashMap<EnemyType, EnemyBaseData>();

        foreach (KeyValuePair<EnemyType, EnemyBaseData> entry in dataBaseEnemies)
        {
            if (entry.Value != null)
                enemiesByType.Add(entry.Key, entry.Value);
        }
    }

    //-> O(1) promedio
    public EnemyBaseData GetEnemyData(EnemyType type)
    {
        if (enemiesByType == null) BuildLookup();

        if (enemiesByType.TryGetValue(type, out EnemyBaseData data))
            return data;

        Debug.LogWarning($"DataBaseEnemy: no existe data para el tipo {type}");
        return null;
    }

    public void RefreshLookup() => BuildLookup();
}