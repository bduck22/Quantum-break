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

public class EnemyController : MobBase
{
    [Header("무기")]
    public MobWeaponBase Weapon;

    [Header("움직임 알고리즘")]
    public MovementAIBase MovementAI;

    public event Action OnAttacked;

    public List<Transform> WayPoints => MovementAI.WayPoints;

    MobEyeChecker EyeChecker;

    //[HideInInspector]
    public PlayerController Player;
    private void Start()
    {
        EnemyInit();
    }

    public void EnemyInit()
    {
        MovementAI = GetComponent<MovementAIBase>();
        Weapon = GetComponent<MobWeaponBase>();
        if (GetComponent<MobEyeChecker>())
        {
            EyeChecker = GetComponent<MobEyeChecker>();
        }

        MovementAI.Init();
        Weapon.Init();
        EyeChecker.Init(Player);
    }

    private void Update()
    {
        if (EyeChecker.LockOn)
        {
            Quaternion targetrotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Player.transform.position - transform.position), Time.deltaTime * 5f);
            targetrotation.x = 0;
            targetrotation.z = 0;
            transform.rotation = targetrotation;
        }

        if (EyeChecker != null)
        {
            if (EyeChecker.CheckPlayerInEye())
            {
                if (MovementAI.IsMoving)
                {
                    MovementAI.OnStop();
                }

                if (Weapon.IsCanAttack())
                {
                    Weapon.OnAttack();
                }
                else
                {
                    Weapon.OnRating();
                }

                return;
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (EyeChecker.LockOn)
        {
            return;
        }

        if (!MovementAI.IsMoving)
        {
            MovementAI.OnStart();
            Weapon.OnStop();
            return;
        }
        MovementAI.OnMove();
    }
}
