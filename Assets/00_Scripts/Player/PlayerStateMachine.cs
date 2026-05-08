using System;
using UnityEngine;

[Serializable]
public class StateMachine
{
    public State CurrentState;

    public void ChangeState(State newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
    }

    public void InitState(State startingState)
    {
        CurrentState = startingState;
    }

}
