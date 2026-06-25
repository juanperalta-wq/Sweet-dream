using UnityEngine;

public class SanityBuff : Buff
{
    public float Amount = 10;

    public SanityBuff(float duration, float amount)
    {
        BuffName = "SanityBuff";
        Duration = duration;
        Amount = amount;
    }
    public override void Apply(PlayerStats entity)
    {
        entity.Sanity += Amount;
        Debug.Log("Apply Sanity Buff");
    }
    public override void Remove(PlayerStats entity)
    {
        //entity.Sanity -= Amount;
    }
}
