using UnityEngine;

public abstract class MobBase : MonoBehaviour
{
    public string MobName;
    public Enemy_Type Type;
    [Header("현재 상태")]
    public Mob_State CurrentState;
    [Header("능력치")]
    public float Speed
    {
        get
        {
            return defaultSpeed + plusSpeed;
        }
        set
        {
            plusSpeed = value;
        }
    }
    public float defaultSpeed;
    [SerializeField]
    private float plusSpeed;

    [Header("격발 피벗")]
    public Transform ShootPoint;
}
