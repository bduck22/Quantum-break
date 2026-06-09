using UnityEngine;

public class SlowBuff : LastingBuff
{
    private void Update()
    {
        base.Update();
    }

    public override void BuffActived()
    {
        if (IsPlayerBuff)
        {
            Player.Speed = -Player.defaultSpeed * Power;
        }
        else
        {
            Enemy.Speed = -Enemy.defaultSpeed * Power;
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
    }
}
