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

public class GroundState : State
{
    public GroundState(PlayerController controller)
    {
        this.Controller = controller;
        StateType = PlayerState.Ground;
    }

    public override void Move()
    {
        float speed = Controller.Speed;
        Vector3 move = Controller.InputHandler.Move;
        float gravity = Controller.Gravity;
        float yAdd = Controller.YDownAdd;
        Controller.PlayerMovement.Move(new PlayerMovementData(speed, move, gravity, yAdd));
    }

    public override void Jump()
    {
        Controller.PlayerMovement.Jump(Controller.JumpPower);
    }
}

public class AirState : State
{
    public AirState(PlayerController controller)
    {
        this.Controller = controller;
        StateType = PlayerState.Air;
    }

    public override void Move()
    {
        float speed = Controller.Speed * 0.8f;
        Vector3 move = Controller.InputHandler.Move;
        float gravity = Controller.Gravity;
        float yAdd = Controller.YDownAdd;
        Controller.PlayerMovement.Move(new PlayerMovementData(speed, move, gravity, yAdd));
    }

    public override void Jump()
    {
    }
}

public class WallState : State
{
    public WallState(PlayerController controller)
    {
        this.Controller = controller;
        StateType = PlayerState.Wall;
    }


    public override void Move()
    {
    }
    public override void Jump()
    {
    }
}