using UnityEngine;

public class SlowBuff : LastingBuff
{

    protected override BuffBase CreateCloneInstance()
    {
        return new SlowBuff();
    }

    public override void BuffActived()
    {
        if (IsPlayerBuff)
        {
            Player.Speed = -Player.defaultSpeed * Data.Power;
        }
        else
        {
            Enemy.Speed = -Enemy.defaultSpeed * Data.Power;
        }
    }

    public override void BuffDeactived()
    {
        if (IsPlayerBuff)
        {
            Player.Speed = 0;
        }
        else
        {
            Enemy.Speed = 0;
        }
        TargetExit();
    }
}
