using UnityEngine;

public class SlowDebuffTurret : TurretDebuffBase
{
    public void Init()
    {

    }
    public override void Attack()
    {
        AttackObject.gameObject.SetActive(true);
    }
}
