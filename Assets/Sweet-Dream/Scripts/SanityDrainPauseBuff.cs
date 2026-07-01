using UnityEngine;
// Pausa el drenaje de cordura mientras el buff está activo, y restaura el valor original al salir. Usa la propiedad pública PlayerStats.DrainAmount (ver PlayerStats.cs actualizado).
public class SanityDrainPauseBuff : Buff
{
    private float previousDrainAmount;

    public SanityDrainPauseBuff(float duration)
    {
        BuffName = "SanityDrainPauseBuff";
        Duration = duration;
    }
    public override void Apply(PlayerStats entity)
    {
        previousDrainAmount = entity.DrainAmount;
        entity.DrainAmount = 0f;
        Debug.Log("Apply SanityDrainPauseBuff");
    }
    public override void Remove(PlayerStats entity)
    {
        entity.DrainAmount = previousDrainAmount;
    }
}