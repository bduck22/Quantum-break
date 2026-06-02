using System.Collections.Generic;
using System;
using UnityEngine;
public enum Mob_State
{
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
    [Header("상태")]
    public bool IsDead;
    public bool Invincibility;

    [Header("무기")]
    public MobWeaponBase Weapon;

    [Header("움직임 알고리즘")]
    public MovementAIBase MovementAI;

    public event Action OnAttacked;
    public event Action OnWalked;
    public event Action OnFind;
    public event Action OnDeath;

    public Transform[] WayPoints;

    [Header("허리")]
    public Transform Spine;

    MobEyeChecker EyeChecker;

    [HideInInspector]
    public PlayerController Player;
    [HideInInspector]
    public Transform Core;
    [HideInInspector]
    EnemyPoolManager PoolManager;
    [HideInInspector]
    Collider Collider;

    private void Awake()
    {
        MovementAI = GetComponent<MovementAIBase>();
        Weapon = GetComponent<MobWeaponBase>();
        if (GetComponent<MobEyeChecker>())
        {
            EyeChecker = GetComponent<MobEyeChecker>();
        }
        Collider = GetComponentInChildren<Collider>();
    }
    public void DefaultInit(EnemyPoolManager poolManager, Enemy_Type type)
    {
        this.PoolManager = poolManager;
        this.Type = type;
    }

    public void EnemyInit(Transform[] WayPoints, Vector3 Position)
    {
        DeathTimer = 0;
        IsDead = false;
        Invincibility = false;
        this.WayPoints = (Transform[])WayPoints.Clone();

        MovementAI.Init(this.WayPoints);
        Weapon.Init(Player.GetComponent<PlayerController>().PlayerMovement);
        EyeChecker.Init(Player);
        
        transform.position = Position;
    }

    [Header("시체 사라지는 시간")]
    public float DeathTime;
    float DeathTimer;

    private void Update()
    {
        if (IsDead)
        {
            if (DeathTime > DeathTimer)
            {
                DeathTimer += Time.deltaTime;
            }
            else
            {
                PoolManager.InPool((int)Type, this);
                gameObject.SetActive(false);
                DeathTimer = 0;
            }
            return;
        }

        if (MovementAI.FinalArrived)
        {
            if (CurrentState == Mob_State.Move)
            {
                CurrentState = Mob_State.Attack;
                OnFind?.Invoke();
                MovementAI.OnStop();
                EyeChecker.Target = Core;
                EyeChecker.CheckTargetInEye();
            }

            LookAt(Core.transform);

            Attack();
            return;
        }

        if (EyeChecker.LockOn)
        {
            if(EyeChecker.lockontime == 0)
            {
                LookAt(Player.transform);
            }
        }
        else
        {
            if (CurrentState == Mob_State.Attack)
            {
                CurrentState = Mob_State.Move;
                Spine.localRotation = Quaternion.identity;
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
                if (CurrentState == Mob_State.Move)
                {
                    CurrentState = Mob_State.Attack;
                    OnFind?.Invoke();
                    MovementAI.OnStop();
                }

                Attack();
            }
        }
    }

    void LookAt(Transform Target)
    {
        Quaternion targetrotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Target.transform.position - transform.position), Time.deltaTime * 10f);
        targetrotation.x = 0;
        targetrotation.z = 0;
        transform.rotation = targetrotation;

        float Waist = transform.position.y - Target.transform.position.y;
        Spine.localRotation = Quaternion.Euler(Waist * 3, -Waist * 3, Waist * 3);
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
        if (!IsDead&&!Invincibility)
        {
            CurrentState = Mob_State.Death;

            OnDeath?.Invoke();
            IsDead = true;
            Collider.enabled= false;
        }
        //Debug.Log("사망");
        //gameObject.SetActive(false);
    }
}
