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

    public Transform CameraTransform;

    public CameraShake CamShake;

    public float YVeolocity;

    public bool isHoldingJump;

    public bool IsWall;

    public float HoldTime=0;

    public float MaxGravity;

    public float MinGravity;

    public event Action OnMoveStarted;

    public event Action OnMoveStopped;

    public event Action OnStepped;

    public event Action OnDash;

    [SerializeField] PlayerMovementData Data;

    [SerializeField] Vector3 targetWallRunDir;

    public float WallUpAdd;

    Vector3 Dir;

    public Vector3 WallJump;

    Vector3 MoveVector;

    public Vector3 Velocity;

    public float WallExitAngle;

    public float accel;
    public float decel;

    float wallexittime;

    public bool Dashing;

    public Quaternion DashOrigin;

    public Vector3 DashForce;

    public Transform XAngeCamera;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (IsWall)
        {
            wallexittime = 0.3f;
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

        MoveVector += Dir * Data.Speed;
        MoveVector.y = gravity();
        if (IsWall)
        {
            MoveVector += new Vector3(0,WallUpAdd,0);
        }

        if ((Mathf.Abs(WallJump.x)+Mathf.Abs(WallJump.z)) > 1.5f) 
        {
            MoveVector += WallJump;
            MoveVector += targetWallRunDir * 5;
        }


        Vector3 FinalVel;
        if (cc.isGrounded)
        {
            float rate = Data.Input.sqrMagnitude > 0f ? accel : decel;

            Velocity = Vector3.MoveTowards(Velocity, MoveVector, rate * Time.deltaTime);

            FinalVel = Velocity;
        }
        else
        {
            Velocity = Vector3.zero;

            FinalVel = MoveVector;
        }

        if (Dashing)
        {
            FinalVel.x = FinalVel.y/15f;
            FinalVel.y = FinalVel.y/10f;
            FinalVel.z = FinalVel.z/15f;

            FinalVel += transform.right * Data.Input.x*5f;//(DashOrigin * Vector3.right)
            //Debug.Log(FinalVel);
            //Vector3 NextVector = transform.position + FinalVel * Time.deltaTime;
            //NextVector = (DashOrigin - NextVector);

            //Debug.Log(NextVector);
            //if(NextVector.magnitude > 2)
            //{
            //    FinalVel = Vector3.zero;   
            //}
        }

        if(DashForce != Vector3.zero)
        {
            FinalVel += DashForce;
            DashForce -= DashForce * Time.deltaTime * 7f;
            if(DashForce.magnitude <= 0.5f)
            {
                DashForce = Vector3.zero;
            }
        }

        cc.Move(FinalVel * Time.unscaledDeltaTime);

        MoveVector = Vector3.zero;
    }

    public void WallExit()
    {
        Jump(6);
    }

    public float gravity() {
        if (IsWall)
        {
            float viewAlignment = Vector3.Dot(CameraTransform.forward, targetWallRunDir);

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
        if (YVeolocity < 0 || (!isHoldingJump))
        {
            if (cc.isGrounded)
            {
                if(YVeolocity <= -30f)
                {
                    OnStepped?.Invoke();
                }
                WallJump = Vector3.zero;
                YVeolocity = -1f;
            }
            else
            {
                YVeolocity -= Data.YAdd * Time.deltaTime;
            }
        }
        return YVeolocity = Mathf.Clamp(YVeolocity, MinGravity, MaxGravity);
    }

    public void SetWallData(RaycastHit hit)
    {
        IsWall = true;

        Vector3 wallNormal = hit.normal;
        Vector3 wallDir1 = Vector3.Cross(Vector3.up, wallNormal).normalized;
        Vector3 wallDir2 = -wallDir1;

        Vector3 viewDir = CameraTransform.forward;
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
        Debug.Log("Jump!!!");
        if (IsWall)
        {
            WallJump *= JumpPower*0.8f;
        }
        else
        {
            WallJump = Vector3.zero;
        }

        YVeolocity += JumpPower;
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
        DashForce.y = Yaded*0.55f;
        OnDash?.Invoke();
    }
}
