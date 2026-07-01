using System;
using System.Collections.Generic;
using UnityEngine;
using DulceSueño.Algorithms;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private CircularDoubleLinkedList<IInteractable> InventoryData = new();
    public static event Action<CircularDoubleLinkedList<IInteractable>> OnInventoryChange;
    public static event Action<int> OnItemSelect;

    [SerializeField] public Transform equipPoint;
    [SerializeField] private int slotsMax = 6;

    private ItemPickUp currentEquipped;
    private Node<IInteractable> currentNode;
    private int selectedSlot = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnEnable()
    {
        PlayerInputs.OnSlotSelect += SelectSlot;
        PlayerInputs.OnSlotScroll += ScrollSlot;
        PlayerInputs.OnRemoveItem += RemoveCurrentItem;
        PlayerInputs.OnSortInventory += SortInventory;
    }

    private void OnDisable()
    {
        PlayerInputs.OnSlotSelect -= SelectSlot;
        PlayerInputs.OnSlotScroll -= ScrollSlot;
        PlayerInputs.OnRemoveItem -= RemoveCurrentItem;
        PlayerInputs.OnSortInventory -= SortInventory;
    }

    public bool AddItem(IInteractable item)
    {
        if (InventoryData.Count >= slotsMax)
        {
            Debug.Log("No hay espacio en el inventario!");
            return false;
        }

        InventoryData.Add(item);
        Debug.Log("Item agregado al slot " + InventoryData.Count);

        currentNode = InventoryData.tail;
        selectedSlot = InventoryData.Count - 1;

        OnInventoryChange?.Invoke(InventoryData);
        OnItemSelect?.Invoke(selectedSlot);
        EquipNode(currentNode);

        return true;
    }

    private void RemoveCurrentItem()
    {
        if (currentNode != null)
            RemoveItem(currentNode.Value);
    }

    //-> O(n): recorre la lista enlazada buscando el nodo que contiene el item
    public bool RemoveItem(IInteractable item)
    {
        if (item == null) return false;
        if (InventoryData.Count == 0) return false;

        Node<IInteractable> found = null;
        Node<IInteractable> node = InventoryData.head;
        for (int i = 0; i < InventoryData.Count && node != null; i++)
        {
            if (node.Value == item)
            {
                found = node;
                break;
            }
            node = node.Next;
        }

        if (found == null) return false;

        if (found == currentNode)
        {
            if (currentEquipped != null)
            {
                currentEquipped.OnUnequip();
                currentEquipped.OnDrop(equipPoint.position);
                currentEquipped = null;
            }
        }

        Node<IInteractable> newCurrent = null;
        if (InventoryData.Count > 1)
        {
            newCurrent = (found == currentNode) ? found.Next : currentNode;
            if (newCurrent == found)
                newCurrent = found.Prev;
        }

        InventoryData.RemoveNode(found);

        currentNode = newCurrent;
        selectedSlot = currentNode != null ? GetIndexOfNode(currentNode) : -1;

        OnInventoryChange?.Invoke(InventoryData);
        if (selectedSlot >= 0)
            OnItemSelect?.Invoke(selectedSlot);
        EquipNode(currentNode);

        return true;
    }

    // NUEVO: ordena el inventario por tipo de item y luego por ID, usando Selection Sort.
    // Pensado para un botón de "organizar inventario" en la UI.
    //-> O(n) para extraer los items + O(n^2) del Selection Sort + O(n) para reinsertar.
    // Con slotsMax = 6 el costo real es insignificante; se eligió Selection Sort (en vez
    // de Insertion Sort, que ya se usa en SearchState) para tener los dos algoritmos
    // pedidos por la rúbrica, cada uno resolviendo un problema distinto.
    public void SortInventory()
    {
        if (InventoryData.Count <= 1) return;

        // 1. Extraer todos los items a una lista temporal, recorriendo la lista enlazada
        List<IInteractable> items = new List<IInteractable>();
        Node<IInteractable> node = InventoryData.head;
        for (int i = 0; i < InventoryData.Count; i++)
        {
            items.Add(node.Value);
            node = node.Next;
        }

        // 2. Ordenar por tipo de item y, si empatan, por ID
        SortAlgorithms.SelectionSort(items, (a, b) =>
        {
            ItemData dataA = (a as ItemPickUp)?.itemData;
            ItemData dataB = (b as ItemPickUp)?.itemData;

            if (dataA == null || dataB == null) return 0;

            int typeComparison = dataA.Type.CompareTo(dataB.Type);
            return typeComparison != 0 ? typeComparison : dataA.ID.CompareTo(dataB.ID);
        });

        // 3. Vaciar la lista enlazada actual
        while (InventoryData.Count > 0)
        {
            InventoryData.RemoveNode(InventoryData.head);
        }

        // 4. Reinsertar en el nuevo orden
        foreach (IInteractable item in items)
        {
            InventoryData.Add(item);
        }

        // 5. Reapuntar el nodo seleccionado y avisar a la UI
        currentNode = InventoryData.head;
        selectedSlot = 0;

        OnInventoryChange?.Invoke(InventoryData);
        OnItemSelect?.Invoke(selectedSlot);
        EquipNode(currentNode);
    }
    private void SelectSlot(int index)
    {
        if (InventoryData.Count == 0 || index < 0 || index >= InventoryData.Count)
            return;

        selectedSlot = index;
        currentNode = GetNodeAtIndex(index);

        Debug.Log("Slot seleccionado: " + (selectedSlot + 1));
        OnItemSelect?.Invoke(selectedSlot);
        EquipNode(currentNode);
    }
    private void ScrollSlot(float direction)
    {
        if (InventoryData.Count == 0)
            return;

        if (currentNode == null)
            currentNode = InventoryData.head;

        currentNode = direction > 0 ? currentNode.Prev : currentNode.Next;

        selectedSlot = GetIndexOfNode(currentNode);
        Debug.Log("Slot seleccionado por scroll: " + (selectedSlot + 1));
        OnItemSelect?.Invoke(selectedSlot);
        EquipNode(currentNode);
    }
    private Node<IInteractable> GetNodeAtIndex(int index)
    {
        Node<IInteractable> current = InventoryData.head;
        for (int i = 0; i < index && current != null; i++)
            current = current.Next;
        return current;
    }
    private int GetIndexOfNode(Node<IInteractable> target)
    {
        if (target == null) return -1;

        Node<IInteractable> current = InventoryData.head;
        for (int i = 0; i < InventoryData.Count; i++)
        {
            if (current == target) return i;
            current = current.Next;
        }
        return -1;
    }

    private void EquipNode(Node<IInteractable> node)
    {
        if (currentEquipped != null)
        {
            currentEquipped.OnUnequip();
            currentEquipped = null;
        }

        if (node?.Value is ItemPickUp item)
        {
            currentEquipped = item;
            currentEquipped.OnEquip(equipPoint);
            Debug.Log("Equipado: " + item.itemData.ItemName);
        }
    }
    public IInteractable GetSelectedItem()
    {
        return currentNode?.Value;
    }
    public void ConsumeCurrentItem()
    {
        if (currentEquipped == null) return;
        GameObject toDestroy = currentEquipped.gameObject;
        RemoveItem(currentEquipped);
        Destroy(toDestroy);
    }
}