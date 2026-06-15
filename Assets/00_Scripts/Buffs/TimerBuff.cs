using UnityEngine;

public abstract class TimerBuff : BuffBase
{

    public override void Refresh()
    {
        CurrentDuration = Data.Duration;
        BuffActived();
    }

    protected override void Update()
    {
        if (Template)
        {
            return;
        }

        if (IsFinished || (!Player && !Enemy))
        {
            return;
        }

        if (CurrentDuration <= 0)
        {
            CurrentDuration = 0;
            IsFinished = true;
            BuffDeactived();
        }
        else
        {
            CurrentDuration -= Time.deltaTime;
        }
    }

    public override bool Tick()
    {
        if (!OriginalBuff.gameObject.activeSelf)
        {
            BuffDeactived();
        }

        Update();

        return IsFinished;
    }
}

/*
    



*/