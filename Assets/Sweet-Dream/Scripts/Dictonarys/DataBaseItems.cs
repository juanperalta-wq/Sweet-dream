using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBaseItems", menuName = "Scriptable Objects/DataBaseItems")]
public class DataBaseItems : ScriptableObject
{
    public Dictionary<ItemType, List<BaseItem>> dataBaseItems = new();
}
