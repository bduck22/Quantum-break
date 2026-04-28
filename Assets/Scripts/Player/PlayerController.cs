using System;
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

    public PlayerAttack PlayerAttack;

    [Header("플레이어 능력치")]
    public float Sensitivity;
    public float Speed;

    public float JumpPower;
    public float DashPower;
    public float Gravity;

    public float Stamina;
    public float MaxStamina;

    public float StaminaHealSpeed;

    public int Hp;
    public int MaxHp = 3;
    public int PlusHp;

    [Header("플레이어 보정값")]
    public float GroundCoyoteTime;
    public float WallCoyoteTime;
    public float YDownAdd;
    public float WallRunDistance;
    public float WallCancelAngle;
    public float WallIniputAngle;
    public float JumpBufferTime;
    public float WallFrontCheckDistance;
    public float DashReloadTime;

    public Transform SpawnPoint;

    //플레이어 이벤트
    public event Action OnRightWall;
    public event Action OnLeftWall;
    public event Action OnWalk;
    public event Action OnAir;

    public event Action OnDashing;

    private void Awake()
    {
        InputHandler = GetComponent<InputHandler>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerRotate = GetComponent<PlayerRotate>();
        PlayerAttack = GetComponent<PlayerAttack>();

        cc = GetComponent<CharacterController>();

        groundState = new GroundState(this);
        airState = new AirState(this);
        wallState = new WallState(this);

        StateMachine = new StateMachine();

        PlayerInit(SpawnPoint.position);
    }

    private void Update()
    {
        DefaultControl();

        StateTransitions();

        StaminaHeal();

        if (WallDirection == -1)
        {
            OnRightWall?.Invoke();
        }
        else if (WallDirection == 1)
        {
            OnLeftWall?.Invoke();
        }

        if(WallDirection == 0)
        {
            if (WallCoyoteTimer >= WallCoyoteTime + 0.5f&& PlayerMovement.YVeolocity <= -22)
            {
                OnAir?.Invoke();
            }
            else
            {
                OnWalk?.Invoke();
            }
        }
    }

    public void PlayerInit(Vector3 SpawnPoint)
    {
        transform.position = SpawnPoint;

        InitState();

        StatInit();
    }

    public void StatInit()
    {
        Hp = MaxHp;

        Stamina = MaxStamina;
    }

    void StaminaHeal()
    {
        if (!InputHandler.DashPressed)
        {
            Stamina = Mathf.Clamp(Stamina + StaminaHealSpeed * Time.deltaTime, 0, MaxStamina);
        }
    }

    [SerializeField] float jumpbuffer;

    bool IsCanJump()
    {
        if (!InputHandler.JumpPressed)
        {
            return false;
        }

        if (CurrentState == PlayerState.Ground)
        {
            if (jumpbuffer == 0 && (cc.isGrounded || GroundCoyoteTime <= GroundCoyoteTimer))
            {
                return true;
            }
            if (jumpbuffer != 0 && (cc.isGrounded))
            {
                return true;
            }

            return false;
        }
        else if (CurrentState == PlayerState.Wall)
        {
            return true;
        }

        return false;

        //if (CurrentState == PlayerState.Ground)
        //{
        //    if (jumpbuffer == 0 && (cc.isGrounded || GroundCoyoteTime <= GroundCoyoteTimer))
        //    {
        //        return true;
        //    }
        //    if (jumpbuffer != 0)
        //    {
        //        return true;
        //    }
        //    if (!InputHandler.DashHeld)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //return true;
    }

    void DashUp()
    {
        DashReLoadTimer = DashReloadTime;
        PlayerMovement.DashOrigin = Quaternion.identity;
        PlayerMovement.Dashing = false;
        Time.timeScale = 1f;
        StateMachine.CurrentState.Dash();
        InputHandler.ClearDash();
    }

    bool IsCanAttack()
    {
        if (InputHandler.AttackPressed)
        {
            return true;
        }
        return false;
    }

    public void DefaultControl()
    {
        if (IsCanAttack())
        {
            PlayerAttack.Attack();
            InputHandler.ClearAttack();
        }

        if (InputHandler.Rotate.magnitude > 0)
        {
            PlayerRotate.Rotate(Sensitivity, InputHandler.Rotate);
        }

        if (DashReLoadTimer > 0)
        {
            DashReLoadTimer -= Time.deltaTime;
        }

        if (InputHandler.DashPressed && InputHandler.DashHeld)
        {
            if (DashReLoadTimer > 0)
            {
                InputHandler.ClearDash();
            }
            else
            {
                if ((Stamina > 0 && PlayerMovement.Dashing) || Stamina >= 1)
                {
                    if (!PlayerMovement.Dashing)
                    {
                        Stamina -= 1f;
                        OnDashing?.Invoke();
                        PlayerMovement.DashOrigin = transform.rotation;
                        PlayerMovement.WallExit();
                        PlayerMovement.Dashing = true;
                        Time.timeScale = 0.05f;
                    }
                    Stamina -= Time.unscaledDeltaTime;
                }
                else if (PlayerMovement.Dashing)
                {
                    DashUp();
                }
                else
                {
                    InputHandler.ClearDash();
                }
            }
        }
        else if (InputHandler.DashPressed)
        {
            DashUp();
        }

        PlayerMovement.isHoldingJump = InputHandler.JumpHeld;

        if (jumpbuffer > 0)
        {
            jumpbuffer -= Time.deltaTime;
        }
        else jumpbuffer = 0;


        if (IsCanJump())
        {
            StateMachine.CurrentState.Jump();
            if (CurrentState != PlayerState.Wall) jumpbuffer = JumpBufferTime;
            InputHandler.ClearJump();
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
                if (jumpbuffer > 0)
                {
                    PlayerMovement.gravity();
                    StateMachine.CurrentState.Jump();
                }
            }

            if (CurrentState == PlayerState.Wall)
            {
                jumpbuffer = 0;
            }
            return;
        }
    }

    float GroundCoyoteTimer;
    float WallCoyoteTimer;
    float DashReLoadTimer;

    //상태 변경 
    public void StateTransitions()
    {
        //땅에 닿아있는지 확인
        if (cc.isGrounded)
        {
            //닿았으면 땅 상태로 변경
            GroundCoyoteTimer = 0;
            WallCoyoteTimer = 0;
            StateChange(PlayerState.Ground);
        }
        else
        {
            //땅에서 떨어져도 일정시간 땅 상태로 유지
            GroundCoyoteTimer += Time.deltaTime;
            if (GroundCoyoteTime <= GroundCoyoteTimer)
            {
                //현재 벽을 타고 있는지 확인
                if (IsWall() && !InputHandler.DashHeld)
                {
                    StateChange(PlayerState.Wall);
                }
                else
                {
                    WallCoyoteTimer += Time.deltaTime;
                    if (WallCoyoteTime <= WallCoyoteTimer || InputHandler.DashHeld)
                    {
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
            if (IsRightWall(WallDirection))
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
            Vector3 wallNormal = hit.normal;
            Vector3 wallDir1 = Vector3.Cross(Vector3.up, wallNormal).normalized;
            Vector3 wallDir2 = -wallDir1;

            Vector3 viewDir = Camera.main.transform.forward;
            viewDir.y = 0f;
            viewDir.Normalize();

            float dot1 = Vector3.Dot(viewDir, wallDir1);
            float dot2 = Vector3.Dot(viewDir, wallDir2);

            Vector3 dir = (dot1 > dot2) ? wallDir1 : wallDir2;

            if (Physics.Raycast(RayTransform.position, dir, WallFrontCheckDistance, LayerMask.GetMask("Map")))
            {
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
        else if (!InputHandler.DashHeld)
        {
            if (Physics.Raycast(RayTransform.position, transform.forward, WallFrontCheckDistance, LayerMask.GetMask("Map")))
            {
                return false;
            }
            //벽에 진입하는 상태
            if (Physics.Raycast(RayTransform.position, transform.right * right, WallRunDistance, LayerMask.GetMask("Map")))
            {
                if (Physics.Raycast(transform.position, transform.right * right, out hit, WallRunDistance, LayerMask.GetMask("Map")))
                {

                    //if (right > 0 && InputHandler.Move.x <= 0)
                    //{
                    //    return false;
                    //}
                    //else if (right < 0 && InputHandler.Move.x >= 0)
                    //{
                    //    return false;
                    //}
                    WallDirection = right;

                    //현재 상태가 벽이 아닐 때 상태 변경
                    Walling = true;
                    WallCoyoteTimer = 0;
                    PlayerMovement.SetWallData(hit);//.transform.right*right

                    return true;
                }
            }
        }

        return false;
    }
}