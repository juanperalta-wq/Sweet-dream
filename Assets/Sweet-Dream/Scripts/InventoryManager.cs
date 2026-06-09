using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public CircularDoubleLinkedList<IInteractable> InventoryData = new();
    public static event Action<CircularDoubleLinkedList<IInteractable>> OnInventoryChange;
    public static event Action<int> OnIntemSelect;

    void Start()
    {
        
    }
}
