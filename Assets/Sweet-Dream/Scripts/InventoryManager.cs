using System;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

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

    private void OnEnable()
    {
        PlayerInputs.OnSlotSelect += SelectSlot;
        PlayerInputs.OnSlotScroll += ScrollSlot;
        PlayerInputs.OnRemoveItem += RemoveCurrentItem;
    }

    private void OnDisable()
    {
        PlayerInputs.OnSlotSelect -= SelectSlot;
        PlayerInputs.OnSlotScroll -= ScrollSlot;
        PlayerInputs.OnRemoveItem -= RemoveCurrentItem;
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

    // Remueve el item actualmente equipado
    private void RemoveCurrentItem()
    {
        if (currentNode?.Value is IInteractable item)
            RemoveItem(item);
    }

    // Remueve un item específico del inventario
    public bool RemoveItem(IInteractable item)
    {
        if (item == null) return false;
        if (InventoryData.Count == 0) return false;

        Node<IInteractable> found = null;
        InventoryData.TraverseInOrder(n => { if (n.Value == item) found = n; });

        if (found == null) return false;

        // Si el item a remover es el actualmente equipado, desequipar y soltar
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
            newCurrent = (found == currentNode) ? found.Next : currentNode ?? InventoryData.head;
        }

        InventoryData.RemoveNode(found);

        currentNode = newCurrent;
        selectedSlot = currentNode != null ? GetIndexOfNode(currentNode) : -1;

        OnInventoryChange?.Invoke(InventoryData);
        OnItemSelect?.Invoke(selectedSlot);
        EquipNode(currentNode);

        return true;
    }

    // Selecciona el slot en la posición dada, si el índice es inválido o el inventario esta vacio no hace nada
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

    // El scroll se mueve en la dirección dada, si el inventario esta vacio no hace nada
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

    // Obtiene el nodo en la posición dada, retorna null si el índice es inválido
    private Node<IInteractable> GetNodeAtIndex(int index)
    {
        Node<IInteractable> current = InventoryData.head;
        for (int i = 0; i < index && current != null; i++)
            current = current.Next;
        return current;
    }

    // Obtiene el índice del nodo dado, retorna -1 si el nodo no se encuentra en la lista
    private int GetIndexOfNode(Node<IInteractable> target)
    {
        if (target == null)
            return -1;

        Node<IInteractable> current = InventoryData.head;
        for (int i = 0; i < InventoryData.Count; i++)
        {
            if (current == target)
                return i;
            current = current.Next;
        }
        return -1;
    }

    // Hace el equipamiento del item, si ya hay uno equipado lo desequipa antes de equipar el nuevo
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
            Debug.Log("Equipado: " + item.ItemData.ItemName);
        }
    }

    public IInteractable GetSelectedItem()
    {
        return currentNode?.Value;
    }
}