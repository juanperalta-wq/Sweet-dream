using UnityEngine;

public class ItemPickUp : MonoBehaviour, IInteractable
{
    [SerializeField] public ItemData itemData;
    [SerializeField] private Vector3 equipPosition;
    [SerializeField] private Vector3 equipRotation;

    public void Interact()
    {
        if (itemData.Type == ItemType.Equipment || itemData.Type == ItemType.EasterEgg)
        {
            InventoryManager.Instance.AddItem(this);
        }
    }

    public void OnEquip(Transform equipPoint)
    {
        transform.SetParent(equipPoint);
        transform.localPosition = equipPosition;
        transform.localRotation = Quaternion.Euler(equipRotation);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }
    public void OnDrop(Vector3 equipPoint)
    {
        transform.SetParent(null);
        transform.position = equipPoint;
        gameObject.SetActive(true);
    }
    public void OnUnequip()
    {
        gameObject.SetActive(false);
    }

    public ItemData ItemData => itemData;
}
