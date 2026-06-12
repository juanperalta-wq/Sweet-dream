using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerStats))]
public class BuffManager : MonoBehaviour
{
    public List<Buff> activeBuffs = new();
    public PlayerStats PlayerStats;
    private void Awake()
    {
        PlayerStats = GetComponent<PlayerStats>();
    }
    public void AddBuff(Buff buff)
    {
        Debug.Log("BuffAdded");
        buff.Apply(PlayerStats);
        activeBuffs.Add(buff);
        StartCoroutine(RemoveBuff(buff));
    }
    public IEnumerator RemoveBuff(Buff buff)
    {
        yield return new WaitForSeconds(buff.Duration);

        buff.Remove(PlayerStats);
        activeBuffs.Remove(buff);
        Debug.Log("BuffRemoved");
    }
}
