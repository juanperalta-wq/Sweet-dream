using UnityEngine;

public abstract class Buff
{
    public string BuffName;
    public float Duration;
    public abstract void Apply(PlayerStats entity);
    public abstract void Remove(PlayerStats entity);
}
