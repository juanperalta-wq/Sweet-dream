using UnityEngine;

public enum BuffType
{
    None,
    Sanity

}
public class BuffFactory
{
    public static Buff CreateBuff(BuffType type)
    {
        Debug.Log("Apply Buff");
        switch (type)
        {
            case BuffType.Sanity: return new SanityBuff(3, 8);
        }
        return null;
    }
}