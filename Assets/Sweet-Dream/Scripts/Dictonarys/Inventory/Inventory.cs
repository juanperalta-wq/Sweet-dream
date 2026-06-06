using Sirenix.OdinInspector;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [TabGroup("Inventario"), ReadOnly]
    [SerializeField] private DoubleLinkedList<InventorySlot> slots = new();

    [TabGroup("Configuracion")]
    [SerializeField] private int maxSlots = 10;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private bool isOpen = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerInputs.OnInventory += ToggleInventory;
    }

    private void OnDisable()
    {
        PlayerInputs.OnInventory -= ToggleInventory;
    }

    #region AddItem
    public bool AddItem(ItemData item)
    {
        if (item.Stackeable)
        {
            Node<InventorySlot> current = slots.head;
            while (current != null)
            {
                if (current.Value.item.ID == item.ID)
                {
                    current.Value.quantity++;
                    return true;
                }
                current = current.Next;
            }
        }

        if (slots.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno!");
            return false;
        }

        slots.Add(new InventorySlot(item, 1));
        return true;
    }
    #endregion

    #region HasItem
    public bool HasItem(int id)
    {
        Node<InventorySlot> current = slots.head;
        while (current != null)
        {
            if (current.Value.item.ID == id)
                return true;
            current = current.Next;
        }
        return false;
    }
    #endregion

    #region RemoveItem
    public void RemoveItem(int id)
    {
        Node<InventorySlot> current = slots.head;
        while (current != null)
        {
            if (current.Value.item.ID == id)
            {
                if (current.Value.quantity > 1)
                    current.Value.quantity--;
                else
                    slots.RemoveNode(current);
                return;
            }
            current = current.Next;
        }
    }
    #endregion

    #region ToggleInventory
    public void ToggleInventory()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "Inventario abierto" : "Inventario cerrado");
    }
    #endregion
}