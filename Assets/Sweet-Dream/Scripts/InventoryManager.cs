using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public CircularDoubleLinkedList<IInteractable> InventoryData = new();
    public static event Action<CircularDoubleLinkedList<IInteractable>> OnInventoryChange;
    public static event Action<int> OnItemSelect;

    private int StotsMax = 6;

    private void Awake()
    {
        Instance = this;
    }

    public bool AddItem(IInteractable item)
    {
        if (InventoryData.Count >= StotsMax)
        {
            Debug.Log("No hay espacio en el inventario!");
            return false;
        }

        InventoryData.Add(item);
        Debug.Log("Item agregado al slot " + InventoryData.Count);
        OnInventoryChange?.Invoke(InventoryData);
        return true;
    }
}