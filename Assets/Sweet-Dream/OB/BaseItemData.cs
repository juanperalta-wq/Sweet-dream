using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "BaseItemData", menuName = "Scriptable Objects/BaseItemData")]
public class BaseItemData : ScriptableObject
{
    [FoldoutGroup("Item Properties")]
    public bool IsOn;
    [FoldoutGroup("Item Properties")]
    public float battery;
}
