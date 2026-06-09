using UnityEngine;

public enum BuffType
{
    None,
    Speed,
    Slow,
    Poison,
    Force,
    Jump,
    Healt

}
public class BuffFactory
{
    public static Buff CreateBuff(BuffType type)
    {
        Debug.Log("Apply Buff");
        switch (type)
        {
            case BuffType.None: return new SpeedBuff(3, 5);

            case BuffType.Speed: return new SpeedBuff(3, 5);

            case BuffType.Slow: return new SpeedBuff(3, 5);

            case BuffType.Poison: return new SpeedBuff(3, 5);

            case BuffType.Force: return new ForceBuff(3, 10);

            case BuffType.Jump: return new JumpBuff(3, 6);

            case BuffType.Healt: return new HealtBuff(3, 9);
        }
        return null;
    }
}