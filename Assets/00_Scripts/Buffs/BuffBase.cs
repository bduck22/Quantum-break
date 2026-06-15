using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    public bool IsFinished;

    public DebuffData Data;

    public float CurrentDuration;

    public EnemyController Enemy;

    public PlayerController Player;

    [HideInInspector]
    public BuffBase OriginalBuff;

    public bool IsPlayerBuff;

    public bool Template;

    public void SetTarget(EnemyController enemy)
    {
        Enemy = enemy;
        IsPlayerBuff = false;
        IsFinished = false;
    }
    public void SetTarget(PlayerController player)
    {
        Player = player;
        IsPlayerBuff = true;
        IsFinished = false;
    }

    public BuffBase Clone()
    {
        BuffBase clone = CreateCloneInstance();

        clone.CopyFrom(this, IsPlayerBuff);

        return clone;
    }

    protected abstract BuffBase CreateCloneInstance();

    protected virtual void CopyFrom(BuffBase original, bool originalTarget)
    {
        Data = original.Data;
        IsPlayerBuff = originalTarget;
        if (IsPlayerBuff)
        {
            Player = original.Player;
        }
        else
        {
            Enemy = original.Enemy;
        }
    }

    protected abstract void Update();

    public abstract bool Tick();

    public virtual bool TargetEnter()
    {
        return false;
    }
    public virtual bool TargetExit()
    {
        return false;
    }

    public virtual void BuffActived() { }
    public virtual void BuffDeactived() { }
    public abstract void Refresh();
}

public enum Buff_Type
{
    Slow
}