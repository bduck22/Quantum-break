using JetBrains.Annotations;
using System;
using System.Buffers.Text;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[Serializable]
public struct PlayerMovementData
{
    public PlayerMovementData(float Speed, Vector3 Input, float Gravity, float YAdd, float DashPower)
    {
        this.Speed = Speed;
        this.Input = Input;
        this.Gravity = Gravity;
        this.YAdd = YAdd;
        this.DashPower = DashPower;
    }
    public float Speed;
    public Vector3 Input;
    public float Gravity;
    public float YAdd;
    public float DashPower;
}

public class PlayerMovement : MonoBehaviour
{
    CharacterController cc;
    public bool Stop = true;

    [Header("카메라")]    
    public CameraShake CamShake;

    public Transform XAngeCamera;

    [Header("이전 프레임 이동방향")]
    public Vector3 PreviousVel;

    [Header("현재 중력")]
    public float YVeolocity;

    [Header("이동 상태 확인용")]
    public bool isHoldingJump;

    public bool IsWall;

    public float HoldTime=0;

    public float WallExitAngle;

    [Header("최대/최소 중력")]
    public float MaxGravity;

    public float MinGravity;

    [Header("벽 타기 시 위로 올라가는 값")]
    public float WallUpAdd;

    Vector3 Dir;

    [Header("벽의 바깥쪽 방향")]
    public Vector3 WallJump;

    [Header("현재 미끄러지는 값")]
    public Vector3 Velocity;

    [Header("바닥에서 이동할 때 가속/감속")]
    public float accel;
    public float decel;

    [Header("대시 보정값")]

    public bool Dashing;

    public Vector3 DashOrigin;

    public Vector3 DashForce;

    [Header("점프 상태값")]
    public bool JumpInGround;

    float wallexittime;

    Vector3 MoveVector;

    public event Action OnMoveStarted;

    public event Action OnMoveStopped;

    public event Action OnBigStepped;

    public event Action OnStepped;

    public event Action OnDash;

    [SerializeField] PlayerMovementData Data;

    [SerializeField] Vector3 targetWallRunDir;

    private void Awake()
    {
        blockCheckIgnoreMask = LayerMask.GetMask("Enemy");//LayerMask.GetMask("Player") | LayerMask.GetMask("PlayerMapCol") | LayerMask.GetMask("Map") | LayerMask.GetMask("Bullet") | LayerMask.GetMask("PlayerAttack");
    }
    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Direction();

        MoveVector += Dir * Data.Speed;
        MoveVector.y = gravity();

        WallJumping();

        Vector3 FinalVel;

        Friction(out FinalVel);

        DashMovement(ref FinalVel);

        if (CheckBlocked((FinalVel).normalized))
        {
            MoveVector.x = 0;
            MoveVector.z = 0;
            return;
        }

        PreviousVel = FinalVel;
        if (Time.timeScale == 0.075f)
        {
            FinalVel *= 0.2f;
        }

        if(Time.timeScale == 0 || Stop)
        {
            FinalVel = Vector3.zero;
            OnMoveStopped?.Invoke();
        }

        cc.Move(FinalVel * Time.unscaledDeltaTime);

        MoveVector = Vector3.zero;
    }
    void DashMovement(ref Vector3 FinalVel)
    {
        if (Dashing)
        {
            FinalVel.x = FinalVel.y / 15f;
            FinalVel.y = FinalVel.y / 10f;
            FinalVel.z = FinalVel.z / 15f;

            FinalVel += transform.right * Data.Input.x * 15f;//
            Vector3 NextVector = transform.position + FinalVel * Time.deltaTime;
            NextVector.y = 0;
            Vector3 DashOrigin = this.DashOrigin;
            DashOrigin.y = 0;
            NextVector = (DashOrigin - NextVector);

            if (NextVector.magnitude > 10f)
            {
                Vector3 next = NextVector;
                NextVector = transform.position + FinalVel * Time.deltaTime * -1;
                NextVector.y = 0;
                DashOrigin = this.DashOrigin;
                DashOrigin.y = 0;
                NextVector = (DashOrigin - NextVector);

                if (next.magnitude > NextVector.magnitude)
                {
                    FinalVel = Vector3.zero;
                }
            }
        }

        if (DashForce != Vector3.zero)
        {
            FinalVel += DashForce;
            DashForce -= DashForce * Time.deltaTime * 7f;
            if (DashForce.magnitude <= 0.5f)
            {
                DashForce = Vector3.zero;
            }
        }
    }
    void Friction(out Vector3 FinalVel)
    {
        if ((Mathf.Abs(WallJump.x) + Mathf.Abs(WallJump.z)) <= 1.5f&&!IsWall)//cc.isGrounded)
        {
            float rate = Data.Input.sqrMagnitude > 0f ? accel : decel;

            Velocity = Vector3.MoveTowards(Velocity, MoveVector, rate * Time.unscaledDeltaTime);

            Velocity.y = MoveVector.y;

            FinalVel = Velocity;

            if (Velocity.magnitude <= 1.1f)
            {
                Velocity = Vector3.zero;
            }
        }
        else
        {
            Velocity = Vector3.zero;

            FinalVel = MoveVector;
        }
    }

    void WallJumping()
    {
        if (IsWall)
        {
            MoveVector += new Vector3(0, WallUpAdd, 0);
        }

        if ((Mathf.Abs(WallJump.x) + Mathf.Abs(WallJump.z)) > 1.5f)
        {
            MoveVector += WallJump;
            MoveVector += targetWallRunDir * 5;
        }
    }

    void Direction()
    {
        if (IsWall)
        {
            wallexittime = 0.15f;
            Dir = targetWallRunDir;
        }
        else
        {
            if (wallexittime > 0)
            {
                wallexittime -= Time.deltaTime;
            }
            else
            {
                if (Dashing)
                {
                    wallexittime = 0;
                    Dir = Vector3.zero;
                }
                else
                {
                    Dir = transform.forward * Data.Input.z + transform.right * Data.Input.x;
                }
            }
        }

        Dir.y = 0;
    }

    private int blockCheckIgnoreMask;
    private static readonly Vector3 BlockRayOffset = new Vector3(0f, 0.4f, 0f);

    public void VelocityInit()
    {
        Velocity = Vector3.zero;
    }

    bool CheckBlocked(Vector3 dir)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + BlockRayOffset, dir, out hit,  3, blockCheckIgnoreMask))
        {
            Vector3 hitP = hit.transform.position;
            Vector3 distance = hitP - transform.position;
            //distance.y = 0;
            if(distance.sqrMagnitude <= 1.65f)
            {
                return true;
            }
        }
        return false;
    }

    public void WallExit()
    {
        Jump(6);
    }

    private bool wasGrounded;

    public float gravity() {
        if (IsWall)
        {
            float viewAlignment = Vector3.Dot(XAngeCamera.forward, targetWallRunDir);

            float minDot = Mathf.Cos(WallExitAngle * Mathf.Deg2Rad);

            if (viewAlignment >= minDot)
            {
                YVeolocity = 0;
                return YVeolocity;
            }
            else
            {
                WallExit();
            }
        }
        YVeolocity -= Data.Gravity * Time.deltaTime;
        if (YVeolocity < 4f || (!isHoldingJump))
        {
            if (cc.isGrounded)
            {
                if (!wasGrounded)
                {
                    if (YVeolocity <= -25f)
                    {
                        OnBigStepped?.Invoke();
                    }
                    OnStepped?.Invoke();
                }
                JumpInGround = false;
                WallJump = Vector3.zero;
                YVeolocity = -1f;
            }
            else
            {
                YVeolocity -= Data.YAdd * Time.deltaTime;
            }

            wasGrounded = cc.isGrounded;
        }
        return YVeolocity = Mathf.Clamp(YVeolocity, MinGravity, MaxGravity);
    }

    public void SetWallData(RaycastHit hit)
    {
        Vector3 wallNormal = hit.normal;
        Vector3 wallDir1 = Vector3.Cross(Vector3.up, wallNormal).normalized;
        Vector3 wallDir2 = -wallDir1;

        Vector3 viewDir = XAngeCamera.forward;
        viewDir.y = 0f;
        viewDir.Normalize();

        float dot1 = Vector3.Dot(viewDir, wallDir1);
        float dot2 = Vector3.Dot(viewDir, wallDir2);

        targetWallRunDir = (dot1 > dot2) ? wallDir1 : wallDir2;

        WallJump = new Vector3(wallNormal.x, 0, wallNormal.z);
    }

    public void Move(PlayerMovementData Data)
    {
        Data.Input = new Vector3(Data.Input.x, 0, Data.Input.y);
        if (this.Data.Input != Data.Input || this.Data.Gravity != Data.Gravity)
        {
            this.Data = Data;

            if (Data.Input == Vector3.zero && !IsWall)
            {
                OnMoveStopped?.Invoke();
            }
            else
            {
                OnMoveStarted?.Invoke();
            }
        }
    }

    public void Jump(float JumpPower)
    {
        if (IsWall)
        {
            WallJump *= JumpPower;
        }
        else 
        {
            if (JumpInGround)
            {
                return;
            }

            JumpInGround = true;
            WallJump = Vector3.zero;
        }

        YVeolocity = JumpPower*0.85f;
        IsWall = false;
    }

    public void Dash()
    {
        DashForce = Vector3.zero;
        float Yaded=0;
        if (XAngeCamera.eulerAngles.x <= 330f && XAngeCamera.eulerAngles.x >= 280f)
        {
            Yaded = (XAngeCamera.eulerAngles.x - 180);
        }
        else if (XAngeCamera.eulerAngles.x <= 80 && XAngeCamera.eulerAngles.x >= 30f)
        {
            Yaded = (XAngeCamera.eulerAngles.x + 70);
        }
        DashForce = transform.forward * (Data.DashPower-Yaded/2f);
        if(XAngeCamera.eulerAngles.x <= 180)
        {
            Yaded = -Yaded;
        }
        if(YVeolocity < 0) YVeolocity = 0;
        DashForce.y = Yaded*0.7f;
        OnDash?.Invoke();
    }

    public void AttackDash()
    {
        //DashForce += transform.forward * 12.5f;
    }
}
