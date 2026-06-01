using UnityEngine;


[CreateAssetMenu(menuName = "Data/Mob/WeaponData")]
public class MobWeaponData : ScriptableObject
{
    public float BulletSpeed;
    public float ShootRate;
    public int BulletCount;
    public float BulletDelay;
}