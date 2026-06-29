using UnityEngine;

public class RushWeapon : MobWeaponBase
{
    PlayerMovement playermove;
    public EnemyController Enemy;
    public override void Init(PlayerMovement playermove, EnemyController enemy)
    {
        this.playermove = playermove;
        Enemy = enemy;
        RatingTime = Data.ShootRate * 0.7f;
    }
    public override bool IsCanAttack()
    {
        if (RatingTime >= Data.ShootRate - 1.3f && Shooted)
        {
            Shooted = false;
            //ShinyEffect.Play();
        }
        return (RatingTime >= Data.ShootRate);
    }

    public override void OnAttack(Transform ShootP, Transform Target)
    {
        RatingTime = 0;
        Shooted = true;
        Target.GetComponent<MainCoreController>().Hit();
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
