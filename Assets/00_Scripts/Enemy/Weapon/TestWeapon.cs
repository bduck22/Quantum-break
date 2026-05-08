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

    public override void OnAttack()
    {
        RatingTime = 0;
        Debug.Log("투명 공격");
    }
    public override void OnRating()
    {
        RatingTime += Time.deltaTime;
        RatingTime = Mathf.Clamp(RatingTime, 0, Data.ShootRate);
        Debug.Log("대기 중");
    }
    public override void OnStop()
    {
        RatingTime = Data.ShootRate / 2f;
        Debug.Log("사격 중지");
    }
}