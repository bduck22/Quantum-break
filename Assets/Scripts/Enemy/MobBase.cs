using UnityEngine;

public enum Mob_State
{
    Move,
    Stop,
    Attack,
    Death
}

public abstract class MobBase : MonoBehaviour
{
    public string MobName;
    [Header("현재 상태")]
    public Mob_State CurrentState;
    [Header("능력치")]
    public MobStatData StatData;
    [Header("인식 관련")]
    public Transform Head;
    [Header("격발 피벗")]
    public Transform ShootPoint;
}
