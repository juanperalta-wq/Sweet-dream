using UnityEngine;

public class JumpBuff : Buff
{
    public float Amount = 10;
    public JumpBuff(float duration, float amount)
    {
        BuffName = "JumpBuff";
        Duration = duration;
        Amount = amount;
    }
    public override void Apply(BaseEntity entity)
    {
        entity.JumpForce += Amount;
        Debug.Log("Apply Jump Buff");

    }
    public override void Remove(BaseEntity entity)
    {
        entity.JumpForce -= Amount;
    }
}
