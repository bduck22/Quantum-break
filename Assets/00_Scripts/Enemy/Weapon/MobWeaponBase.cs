using UnityEngine;

public interface MobWeapon
{
    public bool IsCanAttack();
    public void OnAttack(Transform ShootP, Transform Target);
    public void OnRating();
    public void OnStop();
    public void Init(PlayerMovement playermove, EnemyController enemy);
}

public abstract class MobWeaponBase : MonoBehaviour, MobWeapon
{
    public float RatingTime;
    public MobWeaponData Data;
    public ParticleSystem ShinyEffect;
    public bool Shooted;
    public bool Dead;
    public abstract bool IsCanAttack();
    public abstract void OnAttack(Transform ShootP, Transform Target);
    public abstract void OnRating();
    public abstract void OnStop();
    public abstract void Init(PlayerMovement playermove, EnemyController enemy);
}
