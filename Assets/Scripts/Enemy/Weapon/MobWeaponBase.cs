using UnityEngine;

public interface MobWeapon
{
    public bool IsCanAttack();
    public void OnAttack();
    public void OnRating();
    public void OnStop();
    public void Init();
}

public abstract class MobWeaponBase : MonoBehaviour, MobWeapon
{
    public MobWeaponData Data;
    public abstract bool IsCanAttack();
    public abstract void OnAttack();
    public abstract void OnRating();
    public abstract void OnStop();
    public abstract void Init();
}
