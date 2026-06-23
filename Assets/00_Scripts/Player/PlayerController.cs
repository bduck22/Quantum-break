using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-50)]
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
    public bool IsHologram;
    public bool IsDead;
    public bool Stop = true;

    [Header("벽 타기 관련")]
    public bool Walling;

    public int WallDirection;

    public Transform RayTransform;

    [Header("동작 실행 스크립트")]
    public PlayerMovement PlayerMovement;

    public PlayerRotate PlayerRotate;

    public ArmAnimationController ArmAnimationController;

    public UIController UIController;

    [Header("플레이어 능력치")]
    public float Speed
    {
        get
        {
            return defaultSpeed + plusSpeed;
        }
        set
        {
            plusSpeed = value;
        }
    }
    public float defaultSpeed;
    [SerializeField]
    private float plusSpeed;

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
    public float InvincibilityTime;


    float AttackBufferTimer;

    [Header("상태값")]
    public bool CanWalkJump;

    public bool Invincibility;

    [Header("게임 설정치")]
    public float Sensitivity;

    //플레이어 이벤트
    public event Action OnRightWall;
    public event Action OnLeftWall;
    public event Action OnWalk;
    public event Action OnAir;
    public event Action OnAttack;

    public event Action OnGroundJump;
    public event Action OnWallJump;

    public event Action OnDashing;

    public event Action OnHit;
    public event Action EndHitInvincibility;

    public event Action OnParried;

    private void Awake()
    {
        InputHandler = GetComponent<InputHandler>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerRotate = GetComponent<PlayerRotate>();
        ArmAnimationController = GetComponentInChildren<ArmAnimationController>();

        UIController = GetComponentInChildren<UIController>();
        UIController.OpenedUI += OpenedUI;
        UIController.ClosedUI += CloseUI;
        cc = GetComponent<CharacterController>();

        groundState = new GroundState(this);
        airState = new AirState(this);
        wallState = new WallState(this);

        StateMachine = new StateMachine();

        mapMask = LayerMask.GetMask("Map");

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (IsDead|| Time.timeScale == 0 || Stop)
        {
            return;
        }

        DefaultControl();

        StaminaHeal();

        InvincibilityTimerPlay();
    }

    private void LateUpdate()
    {
        StateTransitions();

        ArmMotion();
    }

    public void Parring()
    {
        OnParried?.Invoke();
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
            if (WallCoyoteTimer == 0 && PlayerMovement.YVeolocity <= -5f&&!cc.isGrounded)
            {
                OnAir?.Invoke();
            }
            else
            {
                OnWalk?.Invoke();
            }
        }
    }

    void OpenedUI()
    {
        IsHologram = true;
    }
    void CloseUI()
    {
        IsHologram = false;
    }

    public void PlayerInit(Vector3 spawnPoint)
    {
        cc.enabled = false;

        transform.position = spawnPoint;

        cc.enabled = true;

        InitState();

        StatInit();
    }

    void StatInit()
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
        Invincibility = false;
        UIController.CantOpen = false;
        Time.timeScale = 1f;
        StateMachine.CurrentState.Dash();
        InputHandler.ClearDash();
    }

    bool IsCanAttack()
    {
        if (IsHologram)
        {
            AttackBufferTimer = 0;
            InputHandler.ClearAttack();
            return false;
        }

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
            //if (Invincibility)
            //{
            //    AttackBufferTimer -= Time.unscaledDeltaTime;
            //}
            //else
            //{
                AttackBufferTimer = 0;
                InputHandler.ClearAttack();
                return true;
            //}
        }
        if (InputHandler.AttackPressed)
        {
            //if (Invincibility)
            //{
            //    InputHandler.ClearAttack();
            //    AttackBufferTimer = AttackBufferTime;
            //    return false;
            //}
            return true;
        }
        return false;
    }

    void PressingDash()
    {
        if (DashReLoadTimer > 0)
        {
            InputHandler.ClearDash();
        }
        else
        {
            if ((Stamina > 0 && Invincibility) || Stamina >= 1)
            {
                if (!Invincibility)
                {
                    OnDashing?.Invoke();
                    Stamina -= 1f;
                    Vector3 dashorigin = transform.position;
                    dashorigin.y = 0;
                    PlayerMovement.DashOrigin = dashorigin;
                    PlayerMovement.WallExit();
                    PlayerMovement.Dashing = true;
                    Invincibility = true;
                    UIController.CantOpen = true;
                    Time.timeScale = 0.05f;
                }
                Stamina -= Time.unscaledDeltaTime;
            }
            else if (Invincibility)
            {
                DashUp();
            }
            //else
            //{
            //    InputHandler.ClearDash();
            //}
        }
    }

    void DefaultControl()
    {

        if (IsCanAttack())
        {
            OnAttack?.Invoke();
            //PlayerMovement.AttackDash();
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

        if (IsHologram||Time.timeScale==0.2f)
        {
            InputHandler.ClearDash();
        }
        else if (InputHandler.DashPressed && InputHandler.DashHeld)
        {
            PressingDash();
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
            PlayerState jumpState = CurrentState;

            StateMachine.CurrentState.Jump();

            if (jumpState == PlayerState.Ground)
            {
                OnGroundJump?.Invoke();
            }
            else if (jumpState == PlayerState.Wall)
            {
                OnWallJump?.Invoke();
            }

            InputHandler.ClearJump();
        }

        StateMachine.CurrentState.Move();
    }

    void InitState()
    {
        CurrentState = PlayerState.Ground;
        StateMachine.InitState(groundState);
        Stop = false;
        PlayerMovement.Stop = false;
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
    [SerializeField] float WallCoyoteTimer;
    float DashReLoadTimer;
    float InvincibilityTimer;

    //상태 변경 
    void StateTransitions()
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
                    if(CurrentState == PlayerState.Wall && WallCoyoteTime > WallCoyoteTimer && !InputHandler.DashHeld)
                    {
                        WallCoyoteTimer += Time.deltaTime;

                        Walling = false;
                        if (PlayerMovement.IsWall)
                        {
                            PlayerMovement.WallExit();
                            OnWallJump?.Invoke();
                        }

                    }
                    else 
                    {
                        if (PlayerMovement.IsWall)
                        {
                            PlayerMovement.WallExit();
                            OnWallJump?.Invoke();
                        }
                        WallCoyoteTimer = 0;
                        Walling = false;
                        WallDirection = 0;
                        StateChange(PlayerState.Air);
                    }
                }
            }
        }
    }

    bool IsWall()
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
    private readonly Vector3 WallBoxHalfExtents = new Vector3(0.65f, 0.2f, 1.25f);


    //private void OnDrawGizmos()
    //{

    //    Vector3 center = transform.position + transform.right * 1.5f;
    //    Quaternion rotation = transform.rotation;

    //    Gizmos.color = Color.red;
    //    Matrix4x4 oldMatrix = Gizmos.matrix;
    //    Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);

    //    Gizmos.DrawWireCube(Vector3.zero, WallBoxHalfExtents * 2f);

    //    Gizmos.matrix = oldMatrix;
    //}

    bool IsRightWall(int right)
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

            Vector3 boxCenter = transform.position + transform.right * right * 1.5f;// * (WallRunDistance+1);

            int hitCount = Physics.OverlapBoxNonAlloc(
                boxCenter,
                WallBoxHalfExtents,
                wallHits,
                transform.rotation,
                mapMask
            );

            RaycastHit hit2;
            if (Physics.Raycast(transform.position, transform.right * right, out hit2, (WallRunDistance + 1), mapMask) && PlayerMovement.IsWall)
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
                //벽 반대 방향으로 누르면 해제
                //if (right > 0 && InputHandler.Move.x < 0)
                //{
                //    return false;
                //}
                //else if (right < 0 && InputHandler.Move.x > 0)
                //{
                //    return false;
                //}

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (!InputHandler.DashHeld)
        {
            if (Physics.Raycast(RayTransform.position, transform.forward, WallFrontCheckDistance, mapMask))
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

    void InvincibilityTimerPlay()
    {
        if (Invincibility&&!PlayerMovement.Dashing)
        {
            if(InvincibilityTimer > 0)
            {
                DashReLoadTimer = DashReloadTime;
                InvincibilityTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                InvincibilityTimer = 0;
                Invincibility = false;
                UIController.CantOpen = false;
                DashReLoadTimer = DashReloadTime;
                EndHitInvincibility?.Invoke();
            }
        }
    }

    public void OnHited()
    {
        Invincibility = true;
        UIController.CantOpen = true;
        InvincibilityTimer = InvincibilityTime;
        PlayerMovement.VelocityInit();
        if(PlusHp > 0)
        {
            PlusHp--;
        }
        else
        {
            Hp--;
        }

        if(Hp <= 0)
        {
            Dead();
            return;
        }

        OnHit?.Invoke();
    }

    public void Dead()
    {
        IsDead = true;
        PlayerMovement.Stop = true;
    }
}