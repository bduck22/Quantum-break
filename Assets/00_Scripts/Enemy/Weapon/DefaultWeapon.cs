using System.Collections;
using UnityEngine;

public class DefaultWeapon : MobWeaponBase
{
    PlayerMovement playermove;

    public float LeadPer;

    public Vector3 TargetPivot;

    public EnemyController Enemy;
    public override void Init(PlayerMovement playermove, EnemyController enemy)
    {
        this.playermove = playermove;
        Enemy = enemy;
        RatingTime = Data.ShootRate * 0.7f;
    }
    public override bool IsCanAttack()
    {
        if (RatingTime >= Data.ShootRate - 1.3f &&Shooted)
        {
            Shooted = false;
            ShinyEffect.Play();
        }
        return (RatingTime >= Data.ShootRate);
    }

    public override void OnAttack(Transform ShootP, Transform Target)
    {
        RatingTime = 0;
        Shooted = true;
        Vector3 TargetPos = Target.position + TargetPivot;
        Vector3 dir;
        if (Target.gameObject.layer == 9)
        {
            Vector3 TargetVelocity = playermove.PreviousVel;
            if(TargetVelocity.y == -1 || TargetVelocity.y >= 0)
            {
                TargetVelocity.y = 0;
            }
            else
            {
                TargetVelocity.y = -3f;
            }



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

        SpawnManagers.Instance.Bullet.SpawnBullet(ShootP.position, rotation, Data.BulletSpeed, Data.BulletCount, Data.BulletDelay, Data.DestroyDistance, Enemy);
    }

    public override void OnRating()
    {
        RatingTime += Time.deltaTime;
        RatingTime = Mathf.Clamp(RatingTime, 0, Data.ShootRate);
    }
    public override void OnStop()
    {
        Shooted = true;
    }
}