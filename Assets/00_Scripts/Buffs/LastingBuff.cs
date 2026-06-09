using UnityEngine;

public class LastingBuff : BuffBase
{
    public override void Tick()
    {
        Refresh();
    }

    public override void Refresh()
    {
        if (CurrentDuration==0)
        {
            BuffActived();
        }
        CurrentDuration = Duration;
    }

    public void Update()
    {
        if(IsFinished)
        {
            return;
        }

        CurrentDuration -= Time.deltaTime;
        if (CurrentDuration <= 0)
        {
            CurrentDuration = 0;
            IsFinished = true;
            BuffDeactived();
        }
    }
}
