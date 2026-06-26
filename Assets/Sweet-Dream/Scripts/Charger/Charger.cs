using System.Collections;
using UnityEngine;

public class Charger : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform pointCharger;
    [SerializeField] private float chargeTime = 3f;
    [SerializeField] private float rechargeAmount = 100f;
    [SerializeField] private float moveSpeed = 2f;
    private bool isCharging = false;
    [SerializeField] private PlayerInputs playerInputs;

    public void Interact()
    {
        if (isCharging) return;

        IInteractable selected = InventoryManager.Instance.GetSelectedItem();
        if (selected is not ItemPickUp item) return;

        FlashlightSystem flashlight = item.GetComponent<FlashlightSystem>();
        if (flashlight == null) return;

        StartCoroutine(ChargeRoutine(flashlight, item));
    }

    private IEnumerator ChargeRoutine(FlashlightSystem flashlight, ItemPickUp item)
    {
        isCharging = true;
        playerInputs.enabled = false;

        Transform equipPoint = InventoryManager.Instance.equipPoint;
        if (equipPoint == null)
        {
            playerInputs.enabled = true;
            isCharging = false;
            yield break;
        }

        item.transform.SetParent(null);

        yield return MoveToTarget(item.transform, pointCharger);
        yield return new WaitForSeconds(chargeTime);
        flashlight.Recharge(rechargeAmount);
        yield return MoveToTarget(item.transform, equipPoint);

        item.OnEquip(equipPoint);

        playerInputs.enabled = true;
        isCharging = false;
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