using UnityEngine;

public class HealtBuff : Buff
{
    public float Amount = 10;
    public HealtBuff(float duration, float amount)
    {
        BuffName = "HealtBuff";
        Duration = duration;
        Amount = amount;
    }
    public override void Apply(BaseEntity entity)
    {
        entity.Healt += Amount;
        Debug.Log("Apply Healt Buff");

    }
    public override void Remove(BaseEntity entity)
    {
        entity.Healt -= Amount;
    }
}
