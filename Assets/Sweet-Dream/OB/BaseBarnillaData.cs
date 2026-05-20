using UnityEngine;
using Sirenix.OdinInspector;
[CreateAssetMenu(fileName = "BaseBarnillaData", menuName = "Scriptable Objects/BaseBarnillaData")]
public class BaseBarnillaData : ScriptableObject
{
    [FoldoutGroup("Base Barnilla")]
    public bool recollect;
    [FoldoutGroup("Base Barnilla")]
    public float CordureRestaurated;
}
