using System.Collections.Generic;
using UnityEngine;
using DulceSueño.Collections;

[CreateAssetMenu(fileName = "DataBaseItems", menuName = "Scriptable Objects/DataBaseItems")]
public class DataBaseItems : ScriptableObject
{
    // Se mantiene TAL CUAL para no perder nada de lo que ya llenaste a mano en el Inspector.
    // Unity/Odin no serializan bien un HashMap propio (necesitaría un PropertyDrawer custom),
    // así que este Dictionary sigue siendo solo la herramienta de AUTORÍA en el editor.
    public Dictionary<ItemType, List<ItemData>> dataBaseItems = new();

    // HashMap propio (no System.Collections.Generic): esta es la estructura que
    // realmente usa el juego en tiempo de ejecución para buscar un item por ID.
    private HashMap<int, ItemData> itemsById;

    //-> O(n): recorre una sola vez el Dictionary del Inspector y llena el HashMap.
    // Se llama automáticamente la primera vez que alguien pide un item.
    private void BuildLookup()
    {
        itemsById = new HashMap<int, ItemData>();

        foreach (KeyValuePair<ItemType, List<ItemData>> entry in dataBaseItems)
        {
            foreach (ItemData item in entry.Value)
            {
                if (item != null)
                    itemsById.Add(item.ID, item);
            }
        }
    }

    //-> O(1) promedio gracias al HashMap (antes hubiera sido O(n) recorriendo listas)
    public ItemData GetItemByID(int id)
    {
        if (itemsById == null) BuildLookup();

        if (itemsById.TryGetValue(id, out ItemData item))
            return item;

        Debug.LogWarning($"DataBaseItems: no existe un item con ID {id}");
        return null;
    }

    // Por si el Dictionary se edita en runtime desde el Inspector (Odin) y hay que
    // forzar que el HashMap se reconstruya con los datos actualizados.
    public void RefreshLookup() => BuildLookup();
}