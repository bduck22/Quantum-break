using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement Movement;
    private InputHandler InputHandler;

    private StateMachine stateMachine;

    private void Awake()
    {
        InputHandler = GetComponent<InputHandler>();
        Movement = GetComponent<PlayerMovement>();

        stateMachine = new StateMachine();
    }

    private void Update()
    {
        StateTransitions();
        stateMachine.Update();
    }

    public void StateTransitions()
    {
        if(InputHandler.JumpPressed)
        {
            stateMachine.ChangeState(new JumpState(this));
        }
        else if(InputHandler.DashPressed)
        {
            stateMachine.ChangeState(new DashState(this));
        }
        else if(InputHandler.Move.magnitude > 0)
        {
            stateMachine.ChangeState(new MoveState(this));
        }
    }
}
