using System.Collections;
using UnityEngine;

public class BearCharger : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform mouthPoint;
    [SerializeField] private float chargeTime = 3f;
    [SerializeField] private float rechargeAmount = 100f;
    [SerializeField] private float moveSpeed = 2f;

    public void Interact()
    {
        IInteractable selected = InventoryManager.Instance.GetSelectedItem();
        if (selected is not ItemPickUp item) return;

        FlashlightSystem flashlight = item.GetComponent<FlashlightSystem>();
        if (flashlight == null) return;

        StartCoroutine(ChargeRoutine(flashlight, item.transform, item));
    }

    private IEnumerator ChargeRoutine(FlashlightSystem flashlight, Transform itemTransform, ItemPickUp originalItem)
    {
        Transform equipPoint = InventoryManager.Instance.equipPoint;
        if (equipPoint == null) yield break;

        itemTransform.SetParent(null);

        yield return MoveToTarget(itemTransform, mouthPoint);
        yield return new WaitForSeconds(chargeTime);
        flashlight.Recharge(rechargeAmount);
        yield return MoveToTarget(itemTransform, equipPoint);

        originalItem.OnEquip(equipPoint);
    }

    private IEnumerator MoveToTarget(Transform obj, Transform target)
    {
        while (Vector3.Distance(obj.position, target.position) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(obj.position, target.position, moveSpeed * Time.deltaTime);
            obj.rotation = Quaternion.RotateTowards(obj.rotation, target.rotation, moveSpeed * 180f * Time.deltaTime);
            yield return null;
        }
    }
}