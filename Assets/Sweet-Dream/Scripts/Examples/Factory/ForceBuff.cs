using UnityEngine;

public class ForceBuff: Buff
{
    public float Amount = 10;
    public ForceBuff(float duration, float amount)
    {
        BuffName = "ForceBuff";
        Duration = duration;
        Amount = amount;
    }
    public override void Apply(BaseEntity entity)
    {
        entity.Force += Amount;
        Debug.Log("Apply Force Buff");

    }
    public override void Remove(BaseEntity entity)
    {
        entity.Force -= Amount;
    }
}
