using System.Collections.Generic;
using System;
using UnityEngine;
public enum Mob_State
{
    Stop,
    Move,
    Attack,
    Death
}

public class EnemyInfomation
{
    public EnemyInfomation(float Speed)
    {
        this.Speed = Speed;
        IsAirWind = false;
        IsSlow = false;
        IsStun = false;
    }
    public float Speed;
    public bool IsAirWind;
    public bool IsSlow;
    public bool IsStun;
}

public class EnemyController : MobBase
{
    [Header("정지")]
    public bool IsStop;

    [Header("무기")]
    public MobWeaponBase Weapon;

    [Header("움직임 알고리즘")]
    public MovementAIBase MovementAI;

    public event Action OnAttacked;
    public event Action OnWalked;
    public event Action OnFind;

    public Transform[] WayPoints;

    MobEyeChecker EyeChecker;

    [HideInInspector]
    public PlayerController Player;
    [HideInInspector]
    public Transform Core;
    private void Awake()
    {
        MovementAI = GetComponent<MovementAIBase>();
        Weapon = GetComponent<MobWeaponBase>();
        if (GetComponent<MobEyeChecker>())
        {
            EyeChecker = GetComponent<MobEyeChecker>();
        }
    }

    public void EnemyInit()
    {
        MovementAI.Init(WayPoints);
        Weapon.Init();
        EyeChecker.Init(Player);
    }

    private void Update()
    {
        if (IsStop)
        {
            return;
        }

        if (MovementAI.FinalArrived)
        {
            if (MovementAI.IsMoving)
            {
                MovementAI.OnStop();
                EyeChecker.Target = Core;
                EyeChecker.CheckTargetInEye();
            }

            Attack();
            return;
        }

        if (EyeChecker.LockOn)
        {
            if(EyeChecker.lockontime == 0)
            {
                Quaternion targetrotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), Time.deltaTime * 10f);
                targetrotation.x = 0;
                targetrotation.z = 0;
                transform.rotation = targetrotation;

                float Waist;
                Debug.Log(transform.position.y - )
            }
        }
        else
        {
            if (!MovementAI.IsMoving)
            {
                OnWalked?.Invoke();
                MovementAI.OnStart(new EnemyInfomation(Speed));
                Weapon.OnStop();
                return;
            }
            MovementAI.OnMove();
        }

        if (EyeChecker != null)//여기 수정 필요
        {
            if (EyeChecker.CheckTargetInEye())
            {
                if (MovementAI.IsMoving)
                {
                    OnFind?.Invoke();
                    MovementAI.OnStop();
                }

                Attack();
            }
        }
    }

    void Attack()
    {
        if (Weapon.IsCanAttack())
        {
            OnAttacked?.Invoke();
            Weapon.OnAttack(ShootPoint, EyeChecker.Target);
        }
        else
        {
            Weapon.OnRating();
        }
    }

    public void Hit()
    {
        Debug.Log("사망");
        IsStop = true;
        gameObject.SetActive(false);
    }
}
