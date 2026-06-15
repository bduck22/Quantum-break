using UnityEngine;

public abstract class LastingBuff : BuffBase
{
    public override bool TargetExit()
    {
        IsFinished = true;
        CurrentDuration = 0;
        return true;
    }

    public override void Refresh()
    {
        if (CurrentDuration==0)
        {
            BuffActived();
        }
        CurrentDuration = Data.Duration;
    }

    public override bool Tick()
    {
        if (!OriginalBuff.gameObject.activeInHierarchy)
        {
            BuffDeactived();
        }
        return IsFinished;
    }

    protected override void Update() { }
}
