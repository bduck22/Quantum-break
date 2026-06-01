using System.Collections;
using UnityEngine;

public class TestWeapon : MobWeaponBase
{
    PlayerMovement playermove;

    public float LeadPer;
    public override void Init(PlayerMovement playermove)
    {
        this.playermove = playermove;
    }
    public override bool IsCanAttack()
    {
        return (RatingTime >= Data.ShootRate);
    }

    public override void OnAttack(Transform ShootP, Transform Target)
    {
        RatingTime = 0;
        Vector3 TargetPos = Target.position + new Vector3(0, 1.1f, 0);
        Vector3 dir;
        if (Target.gameObject.layer == 9)
        {
            Vector3 TargetVelocity = playermove.PreviousVel;
            TargetVelocity.y = Mathf.Clamp(TargetVelocity.y, 0, 10000);
            TargetVelocity.y /= 2;

            float Distance = Vector3.Distance(ShootP.position, TargetPos);
            float timeToHit = Distance / Data.BulletSpeed;

            Vector3 predictedPos = TargetPos + TargetVelocity * timeToHit * LeadPer;

            dir = (predictedPos - ShootP.position).normalized;
        }
        else
        {
            dir = (TargetPos - ShootP.position).normalized;
        }

        Quaternion rotation = Quaternion.LookRotation(dir);

        SpawnManagers.Instance.Bullet.SpawnBullet(ShootP.position, rotation, Data.BulletSpeed, Data.BulletCount, Data.BulletDelay);
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