using UnityEngine;

public interface MobWeapon
{
    public bool IsCanAttack();
    public void OnAttack(Transform ShootP, Transform Target);
    public void OnRating();
    public void OnStop();
    public void Init();
}

public abstract class MobWeaponBase : MonoBehaviour, MobWeapon
{
    public float RatingTime;
    public float BulletSpeed;
    public MobWeaponData Data;
    public abstract bool IsCanAttack();
    public abstract void OnAttack(Transform ShootP, Transform Target);
    public abstract void OnRating();
    public abstract void OnStop();
    public abstract void Init();
}
