using System;
using UnityEngine;
public enum Mob_State
{
    Move,
    Attack,
    Death
}

public class EnemyController : MobBase
{
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
            MovementAI?.SpeedRefresh(Speed);
        }
    }
    public float defaultSpeed;
    [SerializeField]
    private float plusSpeed;

    [Header("상태")]
    public bool IsDead;
    public bool Invincibility;

    [Header("무기")]
    public MobWeaponBase Weapon;
    public ParticleSystem ShinyEffect;

    [Header("움직임 알고리즘")]
    public MovementAIBase MovementAI;

    public event Action OnAttacked;
    public event Action OnWalked;
    public event Action OnFind;
    public event Action OnDeath;
    public event Action<EnemyController> OnDead;
    public event Action OnFalse;

    public Transform[] WayPoints;

    [Header("허리")]
    public Transform Spine;

    MobEyeChecker EyeChecker;

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
        Weapon.ShinyEffect = ShinyEffect;
        if (GetComponent<MobEyeChecker>())
        {
            EyeChecker = GetComponent<MobEyeChecker>();
        }
        Collider = GetComponentInChildren<Collider>();

        if (!MovementAI)
        {
            EnemyInit(GameManager.Instance.Player);
        }
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
        Collider.enabled = true;
        this.WayPoints = (Transform[])WayPoints.Clone();

        MovementAI?.Init(this.WayPoints);
        Weapon.Init(Player.GetComponent<PlayerController>().PlayerMovement, this);
        EyeChecker.Init(Player);

        transform.position = Position;

        CurrentState = Mob_State.Attack;
    }

    public void EnemyInit(PlayerController Player)
    {
        GameManager.Instance.CurrentMap.DefaultEnemyCount++;

        DeathTimer = 0;
        IsDead = false;
        Invincibility = false;
        Special = false;
        lastdeadtime = 0;

        this.Player = Player;

        Weapon.Init(Player.PlayerMovement, this);
        EyeChecker.Init(Player);

        OnFind.Invoke();
    }

    [Header("시체 사라지는 시간")]
    public float DeathTime;
    float DeathTimer;


    float lastdeadtime;

    [SerializeField] bool Special;

    bool CheckDead()
    {
        if (IsDead)
        {
            if (Special)
            {
                DeathTimer = DeathTime - 3f;
                if(Time.timeScale == 1)
                {
                    Time.timeScale = 0.2f;
                    lastdeadtime = 1.7f;
                }
                else
                {
                    Special = false;
                }
            }

            if (Time.timeScale == 0.2f && Special)
            {
                lastdeadtime -= Time.unscaledDeltaTime;
                if (lastdeadtime < 0f)
                {
                    Special = false;
                    lastdeadtime = 0;
                    Time.timeScale = 1;
                }
            }

            if (DeathTime > DeathTimer)
            {
                DeathTimer += Time.deltaTime;
            }
            else
            {
                if (MovementAI)
                {
                    GameManager.Instance.CheckWaveEnd();
                    PoolManager.InPool((int)Type, this);
                }
                gameObject.SetActive(false);
                DeathTimer = 0;
                OnFalse?.Invoke();
            }
            return true;
        }
        return false;
    }

    bool CheckMoveFinished()
    {
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
            return true;
        }
        return false;
    }

    bool CheckLockOn()
    {
        if (EyeChecker.LockOn)
        {
            if (EyeChecker.lockontime == 0)
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
                if (MovementAI)
                {
                    OnWalked?.Invoke();
                    MovementAI?.OnStart();
                    MovementAI?.SpeedRefresh(Speed);
                }
                Weapon.OnStop();
                return true;
            }
            MovementAI?.OnMove();
        }
        return false;
    }

    void CheckEye()
    {
        if (EyeChecker != null)//여기 수정 필요
        {
            if (EyeChecker.CheckTargetInEye())
            {
                if (CurrentState == Mob_State.Move)
                {
                    CurrentState = Mob_State.Attack;
                    OnFind?.Invoke();
                    MovementAI?.OnStop();
                }

                Attack();
            }
        }
    }

    private void Update()
    {
        if (CheckDead())
        {
            return;
        }

        if (!MovementAI)
        {
            if (CheckLockOn())
            {
                return;
            }
            CheckEye();
            return;
        }

        if (CheckMoveFinished())
        {
            return;
        }

        if (CheckLockOn())
        {
            return;
        }

        CheckEye();
    }

    void LookAt(Transform Target)
    {
        Quaternion targetrotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Target.transform.position - transform.position), Time.deltaTime * 10f);
        targetrotation.x = 0;
        targetrotation.z = 0;
        transform.rotation = targetrotation;

        float Waist = Target.transform.position.y - transform.position.y;
        float value = 1.5f;
        Spine.localRotation = Quaternion.Euler(-Waist * value, Waist * 0.5f * value, -Waist * value);
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
        if (!IsDead && !Invincibility)
        {
            CurrentState = Mob_State.Death;

            OnDead?.Invoke(this);
            OnDeath?.Invoke();
            IsDead = true;
            Collider.enabled = false;
            if (!MovementAI)
            {
                GameManager.Instance.CurrentMap.DefaultEnemyCount--;

                if (GameManager.Instance.CurrentMap.DefaultEnemyCount == 0)
                {
                    Special = true;
                }
            }
            else
            {
                GameManager.Instance.spawnManagers.Enemy.ImDead();

                if ((GameManager.Instance.spawnManagers.Enemy.EnemyCount == 0 && GameManager.Instance.CurrentMap.IsSpawnEnd()))
                {
                    Special = true;
                }
            }
        }
    }
}
