using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private StateMachine StateMachine;

    [HideInInspector]
    public InputHandler InputHandler;
    [HideInInspector]
    public CharacterController cc;
    [HideInInspector]
    public Vector3 PlayerLookDir => Camera.main.transform.forward;

    public PlayerState CurrentState;

    GroundState groundState;
    AirState airState;
    WallState wallState;

    [Header("동작 실행 스크립트")]
    public PlayerMovement PlayerMovement;

    public PlayerRotate PlayerRotate;

    [Header("플레이어 능력치")]
    public float Sensitivity;
    public float Speed;

    public float JumpPower;
    public float Gravity;

    [Header("플레이어 보정값")]
    public float CoyoteTime;
    public float YDownAdd;
    public float WallRunDistance;
    public Transform RayTransform;

    private void Awake()
    {
        InputHandler = GetComponent<InputHandler>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerRotate = GetComponent<PlayerRotate>();
        cc = GetComponent<CharacterController>();

        groundState = new GroundState(this);
        airState = new AirState(this);
        wallState = new WallState(this);

        StateMachine = new StateMachine();

        StateMachine.InitState(groundState);
    }

    private void Update()
    {
        StateTransitions();

        DefaultControl();

        Debug.Log(PlayerLookDir);
    }

    public void DefaultControl()
    {

        StateMachine.CurrentState.Move();
        

        if (InputHandler.JumpPressed)
        {
            StateMachine.CurrentState.Jump();
            InputHandler.ClearJump();
        }

        PlayerMovement.isHoldingJump = InputHandler.JumpHeld;

        if (InputHandler.DashPressed)
        {
            StateMachine.CurrentState.Dash();
        }

        if(InputHandler.Rotate.magnitude > 0)
        {
            PlayerRotate.Rotate(Sensitivity, InputHandler.Rotate);
        }
    }

    float CoyoteTimer;

    public void StateTransitions()
    {
        if (cc.isGrounded)
        {
            CoyoteTimer = 0;
            StateMachine.ChangeState(groundState);
            CurrentState = PlayerState.Ground;
        }
        else
        {
            CoyoteTimer += Time.deltaTime;
            if(CoyoteTime <= CoyoteTimer)
            {
                RaycastHit hit;
                Debug.DrawRay(RayTransform.position, transform.right);
                if (Physics.Raycast(RayTransform.position, transform.right, out hit , WallRunDistance, LayerMask.GetMask("Map"))){
                    StateMachine.ChangeState(wallState);
                    CurrentState = PlayerState.Wall;
                }
                else if (Physics.Raycast(RayTransform.position, -transform.right, out hit, WallRunDistance, LayerMask.GetMask("Map")))
                {
                    StateMachine.ChangeState(wallState);
                    CurrentState = PlayerState.Wall;
                }
                else
                {
                    StateMachine.ChangeState(airState);
                    CurrentState = PlayerState.Air;
                }
            }
        }
    }
}
