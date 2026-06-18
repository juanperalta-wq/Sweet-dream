using UnityEngine;

public class ItemPickUp : MonoBehaviour, IInteractable
{
    [SerializeField] public ItemData itemData;
    [SerializeField] private Vector3 equipPosition;
    [SerializeField] private Vector3 equipRotation;
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private Rigidbody rb;
    [SerializeField]private Collider coll;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }
    public void Interact()
    {
        if (itemData.Type == ItemType.Equipment || itemData.Type == ItemType.EasterEgg)
        {
            InventoryManager.Instance.AddItem(this);
        }
    }
    // esta funcion se llama desde el InventoryManager para equipar el item
    public void OnEquip(Transform equipPoint)
    {
        rb.isKinematic = true;
        coll.isTrigger = true;
        transform.SetParent(equipPoint);
        transform.localPosition = equipPosition;
        transform.localRotation = Quaternion.Euler(equipRotation);
        transform.localScale = originalScale;
        gameObject.SetActive(true);
    }
    public void OnDrop(Vector3 equipPoint)
    {
        rb.isKinematic = false;
        coll.isTrigger = false;
        transform.SetParent(null);
        transform.position = equipPoint;
        transform.localScale = originalScale;
        gameObject.SetActive(true);
    }
    public void OnUnequip()
    {
        gameObject.SetActive(false);
    }

    public ItemData ItemData => itemData;
}
