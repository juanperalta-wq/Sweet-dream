using UnityEngine;

// Recarga la linterna equipada al instante. Reutiliza el mismo patrón que ya usa Charger.cs
// para encontrar el FlashlightSystem del item seleccionado, así que se integra sin romper nada.
public class FlashlightBuff : Buff
{
    private readonly float rechargeAmount;

    public FlashlightBuff(float duration, float rechargeAmount)
    {
        BuffName = "FlashlightBuff";
        Duration = duration;
        this.rechargeAmount = rechargeAmount;
    }

    public override void Apply(PlayerStats entity)
    {
        if (InventoryManager.Instance == null) return;

        IInteractable selected = InventoryManager.Instance.GetSelectedItem();
        if (selected is not ItemPickUp item) return;

        FlashlightSystem flashlight = item.GetComponent<FlashlightSystem>();
        flashlight?.Recharge(rechargeAmount);

        Debug.Log("Apply FlashlightBuff");
    }

    public override void Remove(PlayerStats entity) { }
}