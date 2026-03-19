using System;
using UnityEngine;

public abstract class State
{
    public PlayerController controller;

    public abstract void Start();
    public abstract void Update();
    public abstract void Exit();
}

public class IdleState : State
{
    public IdleState(PlayerController controller)
    {
        this.controller = controller;
    }

    public override void Start() { }
    public override void Update() {}
    public override void Exit() {}
}

public class JumpState : State
{
    public JumpState(PlayerController controller)
    {
        this.controller = controller;
    }
    public override void Start() { }
    public override void Update() { }
    public override void Exit() { }
}

public class DashState : State
{
    public DashState(PlayerController controller)
    {
        this.controller = controller;
    }
    public override void Start() { }
    public override void Update() { }
    public override void Exit() { }
}

public class MoveState : State
{
    public MoveState(PlayerController controller)
    {
        this.controller = controller;
    }
    public override void Start() { }
    public override void Update() 
    {
        //controller.Mo
    }
    public override void Exit() { }
}

public class FallState : State
{
    public FallState(PlayerController controller)
    {
        this.controller = controller;
    }
    public override void Start() { }
    public override void Update() { }
    public override void Exit() { }
}

public class WallMoveState : State
{
    public WallMoveState(PlayerController controller)
    {
        this.controller = controller;
    }
    public override void Start() { }
    public override void Update() { }
    public override void Exit() { }
}

