using UnityEngine;

public abstract class Buff
{
    public string BuffName;
    public float Duration;
    public abstract void Apply(BaseEntity entity);
    public abstract void Remove(BaseEntity entity);
}
