using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBaseEnemy", menuName = "Scriptable Objects/DataBaseEnemy")]
public class DataBaseEnemy : ScriptableObject
{
    public Dictionary<EnemyType, EnemyBaseData> dataBaseEnemies = new();
}
