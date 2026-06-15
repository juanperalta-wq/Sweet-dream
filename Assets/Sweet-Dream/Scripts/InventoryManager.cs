using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public CircularDoubleLinkedList<IInteractable> InventoryData = new();
    public static event Action<CircularDoubleLinkedList<IInteractable>> OnInventoryChange;
    public static event Action<int> OnItemSelect;

    [SerializeField] public Transform equipPoint;
    private ItemPickUp currentEquipped;
    private Node<IInteractable> currentNode;

    private const int slotsMax = 6;
    private int selectedSlot = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerInputs.OnSlotSelect += SelectSlot;
        PlayerInputs.OnSlotScroll += ScrollSlot;
    }

    private void OnDisable()
    {
        PlayerInputs.OnSlotSelect -= SelectSlot;
        PlayerInputs.OnSlotScroll -= ScrollSlot;
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
        OnInventoryChange?.Invoke(InventoryData);

        currentNode = InventoryData.tail;
        selectedSlot = InventoryData.Count - 1;
        EquipNode(currentNode);

        return true;
    }

    private void SelectSlot(int index)
    {
        selectedSlot = index;
        Debug.Log("Slot seleccionado: " + (selectedSlot + 1));
        OnItemSelect?.Invoke(selectedSlot);

        int i = 0;
        Node<IInteractable> current = InventoryData.head;
        while (current != null)
        {
            if (i == selectedSlot)
            {
                currentNode = current;
                break;
            }
            current = current.Next;
            i++;
        }

        EquipNode(currentNode);
    }

    private void ScrollSlot(float direction)
    {
        if (InventoryData.Count == 0) return;

        if (currentNode == null)
            currentNode = InventoryData.head;

        if (direction > 0)
            currentNode = currentNode.Prev;
        else
            currentNode = currentNode.Next;

        Debug.Log("Slot seleccionado por scroll");
        OnItemSelect?.Invoke(selectedSlot);
        EquipNode(currentNode);
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
            Debug.Log("Equipado: " + item.ItemData.ItemName);
        }
    }

    public IInteractable GetSelectedItem()
    {
        return currentNode?.Value;
    }
}