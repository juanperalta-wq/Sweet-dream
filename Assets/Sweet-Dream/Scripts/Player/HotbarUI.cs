using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] private List<Image> slots;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0);

    private int currentSelected = 0;

    private void OnEnable()
    {
        InventoryManager.OnInventoryChange += UpdateSlots;
        InventoryManager.OnItemSelect += UpdateSelected;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= UpdateSlots;
        InventoryManager.OnItemSelect -= UpdateSelected;
    }

    private void UpdateSlots(CircularDoubleLinkedList<IInteractable> inventory)
    {
        foreach (Image slot in slots)
        {
            slot.sprite = null;
            slot.color = emptyColor;
        }
        Node<IInteractable> current = inventory.head;
        for (int i = 0; i < inventory.Count && i < slots.Count; i++)
        {
            if (current?.Value is ItemPickUp item && item.ItemData.Icon != null)
            {
                slots[i].sprite = item.ItemData.Icon;
                slots[i].color = i == currentSelected ? selectedColor : defaultColor;
            }
            current = current.Next;
        }
    }

    private void UpdateSelected(int index)
    {
        currentSelected = index;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].sprite != null)
                slots[i].color = i == index ? selectedColor : defaultColor;
        }
    }
}