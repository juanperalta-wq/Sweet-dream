using System;
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

    private void RemoveCurrentItem()
    {
        if (currentNode != null)
            RemoveItem(currentNode.Value);
    }
    // Permite remover un item específico del inventario, asegurando que si el item removido es el actualmente equipado, se desequipe y se actualice el slot seleccionado
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
            newCurrent = (found == currentNode) ? found.Next : currentNode ?? InventoryData.head;
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
    // Permite seleccionar un slot específico usando números del 1 al 6, ajustando el índice para que sea 0-based internamente
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
    // Permite cambiar el slot seleccionado usando la rueda del mouse, con scroll hacia arriba para ir al slot anterior y hacia abajo para ir al siguiente
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
    // Devuelve el índice del nodo dado, o -1 si no se encuentra
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
    // Maneja el equipamiento del item seleccionado, asegurando que solo un item está equipado a la vez
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
    // Devuelve el item actualmente seleccionado, o null si no hay ninguno seleccionado
    public IInteractable GetSelectedItem()
    {
        return currentNode?.Value;
    }
    public void ConsumeCurrentItem()
    {
        if (currentEquipped == null) return;
        IInteractable item = currentEquipped;
        RemoveItem(item);
        Destroy(currentEquipped != null ? currentEquipped.gameObject : null);
    }
}