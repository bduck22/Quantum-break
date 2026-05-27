using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private StateMachine StateMachine;

    [HideInInspector]
    public InputHandler InputHandler;
    [HideInInspector]
    public CharacterController cc;
    private Camera mainCamera;

    GroundState groundState;
    AirState airState;
    WallState wallState;

    public RaycastHit hit;

    [Header("현재 상태")]
    public PlayerState CurrentState;

    [Header("벽 타기 관련")]
    public bool Walling;

    public int WallDirection;

    public Transform RayTransform;

    [Header("동작 실행 스크립트")]
    public PlayerMovement PlayerMovement;

    public PlayerRotate PlayerRotate;

    public ArmAnimationController ArmAnimationController;

    [Header("플레이어 능력치")]
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

    public float AttackSpeed;

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
    public float AttackBufferTime;


    float AttackBufferTimer;

    [Header("플레이어 시작 위치")]
    public Transform SpawnPoint;

    [Header("상태값")]
    public bool CanWalkJump;

    [Header("게임 설정치")]
    public float Sensitivity;

    //플레이어 이벤트
    public event Action OnRightWall;
    public event Action OnLeftWall;
    public event Action OnWalk;
    public event Action OnAir;
    public event Action OnAttack;

    public event Action OnDashing;

    private void Awake()
    {
        InputHandler = GetComponent<InputHandler>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerRotate = GetComponent<PlayerRotate>();
        ArmAnimationController = GetComponentInChildren<ArmAnimationController>();

        cc = GetComponent<CharacterController>();

        groundState = new GroundState(this);
        airState = new AirState(this);
        wallState = new WallState(this);

        StateMachine = new StateMachine();

        mapMask = LayerMask.GetMask("Map");

        mainCamera = Camera.main;

        PlayerInit(SpawnPoint.position);
    }

    private void Update()
    {
        DefaultControl();

        StateTransitions();

        StaminaHeal();

        ArmMotion();
    }

    void ArmMotion()
    {
        if (WallDirection == -1)
        {
            OnRightWall?.Invoke();
        }
        else if (WallDirection == 1)
        {
            OnLeftWall?.Invoke();
        }

        if (WallDirection == 0)
        {
            if (WallCoyoteTimer >= WallCoyoteTime + 0.5f && PlayerMovement.YVeolocity <= 2f)
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

    float jumpbuffer;

    bool IsCanJump()
    {
        if (InputHandler.DashHeld || !InputHandler.JumpPressed)
        {
            return false;
        }

        if (CurrentState == PlayerState.Ground)
        {
            if (!PlayerMovement.JumpInGround)
            {
                if ((jumpbuffer == 0))
                {
                    return true;
                }
            }
            //Debug.Log(cc.isGrounded + " and " + jumpbuffer);

            return false;
        }
        else if (CurrentState == PlayerState.Wall && CanWalkJump)
        {
            CanWalkJump = false;
            return true;
        }

        return false;
    }

    void DashUp()
    {
        DashReLoadTimer = DashReloadTime;
        PlayerMovement.DashOrigin = Vector3.zero;
        PlayerMovement.Dashing = false;
        Time.timeScale = 1f;
        StateMachine.CurrentState.Dash();
        InputHandler.ClearDash();
    }

    bool IsCanAttack()
    {
        if (!ArmAnimationController.IsCanAttack)
        {
            if (AttackBufferTimer > 0)
            {
                AttackBufferTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                AttackBufferTimer = 0;
            }

            if (InputHandler.AttackPressed)
            {
                AttackBufferTimer = AttackBufferTime;
            }
            InputHandler.ClearAttack();
            return false;
        }
        else
        {
            ArmAnimationController.SetAttackSpeed(AttackSpeed);
        }

        if (AttackBufferTimer > 0)
        {
            if (PlayerMovement.Dashing)
            {
                AttackBufferTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                AttackBufferTimer = 0;
                InputHandler.ClearAttack();
                return true;
            }
        }
        if (InputHandler.AttackPressed)
        {
            if (PlayerMovement.Dashing)
            {
                InputHandler.ClearAttack();
                AttackBufferTimer = AttackBufferTime;
                return false;
            }
            return true;
        }
        return false;
    }

    public void DefaultControl()
    {
        if (IsCanAttack())
        {
            OnAttack?.Invoke();
            PlayerMovement.AttackDash();
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
                        Vector3 dashorigin = transform.position;
                        dashorigin.y = 0;
                        PlayerMovement.DashOrigin = dashorigin;
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


        if (InputHandler.JumpPressed)
        {
            if (CurrentState == PlayerState.Air)
            {
                jumpbuffer = JumpBufferTime;
                InputHandler.ClearJump();
            }
        }

        if (IsCanJump())
        {
            StateMachine.CurrentState.Jump();
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

                    if (jumpbuffer > 0)
                    {
                        jumpbuffer = 0;
                        PlayerMovement.gravity();
                        StateMachine.CurrentState.Jump();
                    }
                    break;
                case PlayerState.Air:
                    StateMachine.ChangeState(airState);
                    break;
                case PlayerState.Wall:
                    StateMachine.ChangeState(wallState);
                    break;
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
            CanWalkJump = false;
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

        PlayerRotate.WallRotateStop();
        return false;
    }

    private readonly Collider[] wallHits = new Collider[8];
    private int mapMask;
    private static readonly Vector3 WallBoxHalfExtents = new Vector3(0.3f, 0.2f, 0.3f);

    public bool IsRightWall(int right)
    {
        if (Physics.Raycast(transform.position, -transform.up, 1.5f, mapMask))
        {
            return false;
        }

        //벽에 진입하는 상태인지 벽을 타고있는 상태인지에 따라 구분
        if (Walling)
        {
            Vector3 wallNormal = hit.normal;
            Vector3 wallDir1 = Vector3.Cross(Vector3.up, wallNormal).normalized;
            Vector3 wallDir2 = -wallDir1;

            Vector3 viewDir = mainCamera.transform.forward;
            viewDir.y = 0f;
            viewDir.Normalize();

            float dot1 = Vector3.Dot(viewDir, wallDir1);
            float dot2 = Vector3.Dot(viewDir, wallDir2);

            Vector3 dir = (dot1 > dot2) ? wallDir1 : wallDir2;



            RaycastHit fronthit;
            if (Physics.Raycast(RayTransform.position, dir, out fronthit, WallFrontCheckDistance, mapMask))
            {
                float blockDot = Vector3.Dot(fronthit.normal, -dir);
                if (blockDot > 0.6f)
                {
                    return false;
                }
            }

            Vector3 boxCenter = transform.position + transform.right * right * WallRunDistance;

            int hitCount = Physics.OverlapBoxNonAlloc(
                boxCenter,
                WallBoxHalfExtents,
                wallHits,
                transform.rotation,
                mapMask
            );

            RaycastHit hit2;
            if (Physics.Raycast(transform.position, transform.right * right, out hit2, WallRunDistance, mapMask) && PlayerMovement.IsWall)
            {
                if (hit.normal != hit2.normal)
                {
                    float deltaYaw = Vector3.SignedAngle(hit.normal, hit2.normal, Vector3.up);
                    PlayerRotate.WallRotate(deltaYaw);
                    hit = hit2;
                    PlayerMovement.SetWallData(hit);
                }
            }

            //벽에 타고 있는 상태
            if (hitCount > 0)
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
            if (Physics.Raycast(RayTransform.position, transform.forward, WallFrontCheckDistance,   mapMask))
            {
                return false;
            }
            //벽에 진입하는 상태
            if (Physics.Raycast(RayTransform.position, transform.right * right, WallRunDistance,    mapMask))
            {
                if (Physics.Raycast(transform.position, transform.right * right, out hit, WallRunDistance,  mapMask))
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

                    CanWalkJump = true;

                    //현재 상태가 벽이 아닐 때 상태 변경
                    Walling = true;
                    WallCoyoteTimer = 0;
                    PlayerMovement.IsWall = true;
                    PlayerMovement.SetWallData(hit);//.transform.right*right

                    return true;
                }
            }
        }

        return false;
    }

    public void OnHit()
    {
        Hp--;
    }
}