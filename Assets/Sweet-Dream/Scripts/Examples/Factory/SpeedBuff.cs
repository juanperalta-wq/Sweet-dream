using UnityEngine;

public class SpeedBuff : Buff
{
    public float Amount = 5;
    public SpeedBuff(float duration, float amount)
    {
        BuffName = "SpeedBuff";
        Duration = duration;
        Amount = amount;
    }
    public override void Apply(BaseEntity entity)
    {
        entity.Speed += Amount;
        Debug.Log("Apply Speed Buff");

    }
    public override void Remove(BaseEntity entity)
    {
        entity.Speed -= Amount;
    }
}