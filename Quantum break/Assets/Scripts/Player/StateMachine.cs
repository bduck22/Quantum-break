using UnityEngine;

public class StateMachine
{
    [SerializeField] private State currentState;

    public void ChangeState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Start();
    }

    public void InitState(State startingState)
    {
        currentState = startingState;
        currentState.Start();
    }

    public void Update()
    {
        currentState?.Update();
    }
}
