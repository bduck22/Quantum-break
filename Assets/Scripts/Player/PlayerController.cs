using UnityEngine;

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

    public Transform RayTransform;

    GroundState groundState;
    AirState airState;
    WallState wallState;

    public bool Walling;

    public RaycastHit hit;

    public int WallDirection;

    [Header("동작 실행 스크립트")]
    public PlayerMovement PlayerMovement;

    public PlayerRotate PlayerRotate;

    [Header("플레이어 능력치")]
    public float Sensitivity;
    public float Speed;

    public float JumpPower;
    public float Gravity;

    [Header("플레이어 보정값")]
    public float GroundCoyoteTime;
    public float WallCoyoteTime;
    public float AirCoyoteTime;
    public float YDownAdd;
    public float WallRunDistance;
    public float WallCancelAngle;
    public float WallIniputAngle;
    public float JumpBufferTime;
    public float WallFrontCheckDistance;

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

        InitState();
    }

    private void Update()
    {
        DefaultControl();

        StateTransitions();
    }

    [SerializeField] float jumpbuffer;

    bool IsCanJump()
    {
        if (CurrentState == PlayerState.Ground)
        {
            if (jumpbuffer==0&&(cc.isGrounded|| GroundCoyoteTime <= GroundCoyoteTimer))
            {
                return true;
            }
            if(jumpbuffer!=0)
            {
                return true;
            }

            return false;
        }

        return true;
    }

    public void DefaultControl()
    {

        PlayerMovement.isHoldingJump = InputHandler.JumpHeld;

        if(jumpbuffer > 0)
        {
            jumpbuffer -= Time.deltaTime;
        }
        else jumpbuffer = 0;

        if (InputHandler.JumpPressed&&jumpbuffer ==0)
        {
            if (IsCanJump())
            {
                StateMachine.CurrentState.Jump();
                if (CurrentState != PlayerState.Wall) jumpbuffer = JumpBufferTime;
                InputHandler.ClearJump();
            }
        }

        if (InputHandler.DashPressed)
        {
            StateMachine.CurrentState.Dash();
        }

        if (InputHandler.Rotate.magnitude > 0)
        {
            PlayerRotate.Rotate(Sensitivity, InputHandler.Rotate);
        }

        StateMachine.CurrentState.Move();
    }

    void InitState()
    {
        CurrentState = PlayerState.Ground;
        StateMachine.InitState(groundState);
    }

    void StateChange(PlayerState State)
    {
        if (CurrentState != State)
        {
            CurrentState = State;
            switch (State)
            {
                case PlayerState.Ground:
                    StateMachine.ChangeState(groundState);
                    break;
                case PlayerState.Air:
                    StateMachine.ChangeState(airState);
                    break;
                case PlayerState.Wall:
                    StateMachine.ChangeState(wallState);
                    break;
            }
            if (CurrentState == PlayerState.Ground)
            {

                if(jumpbuffer > 0)
                {
                    PlayerMovement.gravity();
                    StateMachine.CurrentState.Jump();
                }

                jumpbuffer = 0;
            }
            return;
        }
    }

    float GroundCoyoteTimer;
    float WallCoyoteTimer;
    float AirCoyoteTimer;

    //상태 변경 
    public void StateTransitions()
    {
        //땅에 닿아있는지 확인
        if (cc.isGrounded)
        {
            //닿았으면 땅 상태로 변경
            GroundCoyoteTimer = 0;
            AirCoyoteTimer = AirCoyoteTime;
            StateChange(PlayerState.Ground);
        }
        else
        {
            //땅에서 떨어져도 일정시간 땅 상태로 유지
            GroundCoyoteTimer += Time.deltaTime;
            if (GroundCoyoteTime <= GroundCoyoteTimer)
            {
                //현재 벽을 타고 있는지 확인
                if (IsWall())
                {
                    if (AirCoyoteTime <= AirCoyoteTimer)
                    {
                        AirCoyoteTimer = 0;
                        StateChange(PlayerState.Wall);
                    }
                }
                else
                {
                    WallCoyoteTimer += Time.deltaTime;
                    if (WallCoyoteTime <= WallCoyoteTimer)
                    {
                        AirCoyoteTimer += Time.deltaTime;
                        if (PlayerMovement.IsWall)
                        {
                            PlayerMovement.WallExit();
                        }
                        Walling = false;
                        WallDirection = 0;
                        StateChange(PlayerState.Air);
                    }
                }
            }
        }
    }

    public bool IsWall()
    {
        if (WallDirection == 0)
        {
            if (IsRightWall(1) || IsRightWall(-1))
            {
                return true;
            }
        }
        else
        {
            if(IsRightWall(WallDirection))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsRightWall(int right)
    {
        if (Physics.Raycast(transform.position, -transform.up, 1.5f, LayerMask.GetMask("Map")))
        {
            return false;
        }

        //벽에 진입하는 상태인지 벽을 타고있는 상태인지에 따라 구분
        if (Walling)
        {
            if (Physics.Raycast(RayTransform.position, transform.forward, WallFrontCheckDistance, LayerMask.GetMask("Map"))){
                return false;
            }

            Vector3 boxCenter = transform.position + transform.right * right * WallRunDistance;

            Vector3 boxHalfExtents = new Vector3(0.3f, 0.8f, 0.3f);

            Collider[] hits = Physics.OverlapBox(
                boxCenter,
                boxHalfExtents,
                transform.rotation,
                LayerMask.GetMask("Map")
            );
            //벽에 타고 있는 상태
            if (hits.Length > 0)
            {
                if (right > 0 && InputHandler.Move.x < 0)
                {
                    return false;
                }
                else if (right < 0 && InputHandler.Move.x > 0)
                {
                    return false;
                }

                return true;
            }
        }
        else
        {
            //벽에 진입하는 상태
            if (Physics.Raycast(RayTransform.position, transform.right * right, WallRunDistance, LayerMask.GetMask("Map")))
            {
                if (Physics.Raycast(transform.position, transform.right * right, out hit, WallRunDistance, LayerMask.GetMask("Map")))
                {

                    if (right > 0 && InputHandler.Move.x <= 0)
                    {
                        return false;
                    }
                    else if (right < 0 && InputHandler.Move.x >= 0)
                    {
                        return false;
                    }
                    WallDirection = right;

                    //현재 상태가 벽이 아닐 때 상태 변경
                    Walling = true;
                    WallCoyoteTimer = 0;
                    PlayerMovement.SetWallData(hit);

                    return true;
                }
            }
        }

        return false;
    }
}