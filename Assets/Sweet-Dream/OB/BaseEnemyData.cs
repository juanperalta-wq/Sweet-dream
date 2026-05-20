using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "BaseEnemyData", menuName = "Scriptable Objects/BaseEnemyData")]
public class BaseEnemyData : ScriptableObject
{
    [FoldoutGroup("Enemy Properties")]
    public float Health;
    [FoldoutGroup("Enemy Properties")]
    public float Speed;
    [FoldoutGroup("Enemy Properties")]
    public float RangeVision;
}
