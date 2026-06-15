using UnityEngine;

public abstract class TurretDebuffBase : TurretAttackBase
{
    public BuffBase DeBuff;

    public override void Init(DebuffData data)
    {
        DeBuff.Data = data;
    }

    public override bool IsCool()
    {
        return !AttackObject.gameObject.activeInHierarchy;
    }
}
