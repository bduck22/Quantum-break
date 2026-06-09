using UnityEngine;

public abstract class TurretAttackBase : MonoBehaviour
{
    public Transform AttackObject;

    public abstract void Attack();
}
