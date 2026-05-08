using UnityEngine;

public abstract class MobBase : MonoBehaviour
{
    public string MobName;
    [Header("현재 상태")]
    public Mob_State CurrentState;
    [Header("능력치")]
    public float Speed;
    [Header("격발 피벗")]
    public Transform ShootPoint;
}
