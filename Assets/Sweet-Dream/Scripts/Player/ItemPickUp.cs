using UnityEngine;

public class ItemPickUp : MonoBehaviour, IInteractable
{
    [SerializeField] public ItemData itemData;
    [SerializeField] private Vector3 equipPosition;
    [SerializeField] private Vector3 equipRotation;
    private Vector3 originalScale;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    private bool isEquipped = false;
    private ObjectDescription objectDescription;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        objectDescription = GetComponent<ObjectDescription>();
    }

    public void Interact()
    {
        if (itemData.Type == ItemType.Equipment || itemData.Type == ItemType.EasterEgg)
            InventoryManager.Instance.AddItem(this);
    }

    public void OnEquip(Transform equipPoint)
    {
        isEquipped = true;
        rb.isKinematic = true;
        coll.isTrigger = true;
        transform.SetParent(equipPoint);
        transform.localPosition = equipPosition;
        transform.localRotation = Quaternion.Euler(equipRotation);
        transform.localScale = originalScale;
        gameObject.SetActive(true);

        if (objectDescription != null) objectDescription.Agarrar();
    }

    public void OnUnequip()
    {
        isEquipped = false;
        transform.SetParent(null);
        gameObject.SetActive(false);

        if (objectDescription != null) objectDescription.Soltar();
    }

    public void OnDrop(Vector3 dropPosition)
    {
        isEquipped = false;
        rb.isKinematic = false;
        coll.isTrigger = false;
        transform.SetParent(null);
        transform.position = dropPosition;
        transform.localScale = originalScale;
        gameObject.SetActive(true);

        if (objectDescription != null) objectDescription.Soltar();
    }

    public ItemData ItemData => itemData;
    public bool IsEquipped => isEquipped;
}