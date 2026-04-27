using System;
using UnityEngine;

public enum PlayerState
{
    Ground,
    Air,
    Wall
}

[Serializable]
public abstract class State
{
    protected PlayerController Controller;

    public PlayerState StateType;

    public abstract void Move();

    public abstract void Jump();

    public virtual void Dash()
    {
        Controller.PlayerMovement.Dash();
    }
}

[Serializable]
public class GroundState : State
{
    public GroundState(PlayerController controller)
    {
        this.Controller = controller;
        StateType = PlayerState.Ground;
    }

    public override void Move()
    {
        Controller.PlayerMovement.Move(new PlayerMovementData(Controller.Speed,
            Controller.InputHandler.Move,
            Controller.Gravity,
            Controller.YDownAdd,
            Controller.DashPower
            ));
    }

    public override void Jump()
    {
        Controller.PlayerMovement.Jump(Controller.JumpPower);
    }
}

[Serializable]
public class AirState : State
{
    public AirState(PlayerController controller)
    {
        this.Controller = controller;
        StateType = PlayerState.Air;
    }

    public override void Move()
    {
        Controller.PlayerMovement.Move(new PlayerMovementData(Controller.Speed,
            Controller.InputHandler.Move,
            Controller.Gravity,
            Controller.YDownAdd,
            Controller.DashPower
            ));
    }

    public override void Jump()
    {
    }
}

[Serializable]
public class WallState : State
{
    public WallState(PlayerController controller)
    {
        this.Controller = controller;
        StateType = PlayerState.Wall;
    }


    public override void Move()
    {
        Controller.PlayerMovement.Move(new PlayerMovementData(Controller.Speed * 1.2f,
            Controller.InputHandler.Move,
            Controller.Gravity,
            Controller.YDownAdd,
            Controller.DashPower
            ));
    }
    public override void Jump()
    {
        Controller.PlayerMovement.Jump(Controller.JumpPower*0.8f);
    }
}