using UnityEngine;

// A diferencia de SanityBuff (instantáneo), este buff restaura cordura de forma GRADUAL
// mientras está activo, usando Tick(). Demuestra que la misma clase base (Buff) puede producir
// comportamientos completamente distintos: eso es lo que hay que explicar como polimorfismo
// en la exposición, no solo "puse override porque lo pedía la rúbrica".
public class SanityRegenBuff : Buff
{
    private readonly float totalAmount;
    private float amountPerSecond;

    public SanityRegenBuff(float duration, float totalAmount)
    {
        BuffName = "SanityRegenBuff";
        Duration = duration;
        this.totalAmount = totalAmount;
    }
    public override void Apply(PlayerStats entity)
    {
        amountPerSecond = Duration > 0f ? totalAmount / Duration : totalAmount;
        Debug.Log("Apply SanityRegenBuff");
    }
    public override void Tick(PlayerStats entity, float deltaTime)
    {
        entity.Sanity += amountPerSecond * deltaTime;
    }
    public override void Remove(PlayerStats entity) { }
}