using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private readonly List<Buff> activeBuffs = new();
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // Sobrecarga #1: agrega un buff ya construido (esto es lo que ya usa Barnilla.cs).
    public void AddBuff(Buff buff)
    {
        buff.Apply(playerStats);
        activeBuffs.Add(buff);
    }

    // Sobrecarga #2: permite crear y aplicar un buff conocido pasando solo strings/floats.
    // Es útil para conectarlo directo desde UnityEvents(botones, MMF Feedbacks), donde Unity
    // no permite pasar un objeto Buff como parámetro serializado en el Inspector.
    public void AddBuff(string buffName, float duration, float amount)
    {
        Buff buff = buffName switch
        {
            "SanityBuff" => new SanityBuff(duration, amount),
            "FlashlightBuff" => new FlashlightBuff(duration, amount),
            "SanityDrainPauseBuff" => new SanityDrainPauseBuff(duration),
            "SanityRegenBuff" => new SanityRegenBuff(duration, amount),
            _ => null
        };

        if (buff == null)
        {
            Debug.LogWarning($"BuffManager: buff '{buffName}' no reconocido.");
            return;
        }

        AddBuff(buff);
    }
    //-> O(n) por frame, donde n = buffs activos (normalmente 0-3, así que el costo real es mínimo)
    private void Update()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];

            buff.Tick(playerStats, Time.deltaTime);
            buff.Duration -= Time.deltaTime;

            if (buff.Duration <= 0f)
            {
                buff.Remove(playerStats);
                activeBuffs.RemoveAt(i);
            }
        }
    }
}