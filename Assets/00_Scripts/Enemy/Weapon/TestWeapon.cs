using UnityEngine;

public class TestWeapon : MobWeaponBase
{
    public override void Init()
    {

    }
    public override bool IsCanAttack()
    {
        return (RatingTime >= Data.ShootRate);
    }

    public override void OnAttack(Transform ShootP, Transform Target)
    {
        RatingTime = 0;
        Vector3 TargetP = Target.position + new Vector3(0,ShootP.localPosition.y-0.5f,0);
        Vector3 dir = (TargetP - ShootP.position).normalized;

        Quaternion rotation = Quaternion.LookRotation(dir);

        BulletObjectPoolManager.instance.SpawnBullet(ShootP.position, rotation, BulletSpeed);
    }
    public override void OnRating()
    {
        RatingTime += Time.deltaTime;
        RatingTime = Mathf.Clamp(RatingTime, 0, Data.ShootRate);
    }
    public override void OnStop()
    {
        RatingTime = Data.ShootRate/2f;
    }
}