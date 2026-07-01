using UnityEngine;

public abstract class Buff
{
    public string BuffName;
    public float Duration;

    public abstract void Apply(PlayerStats entity);
    public abstract void Remove(PlayerStats entity);

    // NUEVO: se llama cada frame mientras el buff está activo. Por defecto no hace nada; solo lo sobreescriben buffs con efecto progresivo, como SanityRegenBuff.
    public virtual void Tick(PlayerStats entity, float deltaTime) { }
}