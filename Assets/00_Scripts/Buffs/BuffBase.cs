using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    public bool IsFinished;

    public float Duration;

    public float CurrentDuration;

    public float Power;

    public EnemyController Enemy;

    public PlayerController Player;

    public bool IsPlayerBuff;

    public void SetTarget(EnemyController enemy)
    {
        IsPlayerBuff = false;
        IsFinished = false;
    }
    public void SetTarget(PlayerController player)
    {
        IsPlayerBuff = true;
        IsFinished = false;
    }

    public virtual void Tick()
    {
        CurrentDuration -= Time.deltaTime;
        if(CurrentDuration <= 0)
        {
            CurrentDuration = 0;
            IsFinished = true;
            BuffDeactived();
        }
    }

    public virtual void Refresh()
    {
        CurrentDuration = Duration;
        BuffActived();
    }

    public virtual void BuffActived() { }
    public virtual void BuffDeactived() { }
}
