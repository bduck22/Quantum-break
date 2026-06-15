using UnityEngine;

public abstract class TurretAttackBase : MonoBehaviour
{
    public ParticleSystem ShootParticle;

    public Transform AttackObject;

    public abstract void Attack(Transform Target);
    public abstract void Init(DebuffData data);

    public abstract bool IsCool();
}
