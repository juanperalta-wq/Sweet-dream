using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public CircularDoubleLinkedList<IInteractable> InventoryData = new();
    public static event Action<CircularDoubleLinkedList<IInteractable>> OnInventoryChange;
    public static event Action<int> OnItemSelect;

    [SerializeField] public Transform equipPoint; // dentro de FirstPersonCamera
    private ItemPickUp currentEquipped;

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

        selectedSlot = InventoryData.Count - 1; // ← Selecciona el último
        EquipSelectedItem();

        return true;
    }

    private void SelectSlot(int index)
    {
        selectedSlot = index;
        Debug.Log("Slot seleccionado: " + (selectedSlot + 1));
        OnItemSelect?.Invoke(selectedSlot);
        EquipSelectedItem();
    }

    private void ScrollSlot(float direction)
    {
        if (direction > 0)
            selectedSlot = (selectedSlot - 1 + slotsMax) % slotsMax;
        else
            selectedSlot = (selectedSlot + 1) % slotsMax;

        Debug.Log("Slot seleccionado: " + (selectedSlot + 1));
        OnItemSelect?.Invoke(selectedSlot);
        EquipSelectedItem();
    }

    private void EquipSelectedItem()
    {
        // desactiva el item actual
        if (currentEquipped != null)
        {
            currentEquipped.OnUnequip();
            currentEquipped = null;
        }

        // obtiene el item del slot seleccionado
        IInteractable selected = GetSelectedItem();

        if (selected is ItemPickUp item)
        {
            currentEquipped = item;
            currentEquipped.OnEquip(equipPoint);
            Debug.Log("Equipado: " + item.ItemData.ItemName);
        }
    }

    public IInteractable GetSelectedItem()
    {
        int i = 0;
        Node<IInteractable> current = InventoryData.head;
        while (current != null)
        {
            if (i == selectedSlot) return current.Value;
            current = current.Next;
            i++;
        }
        return null;
    }
}