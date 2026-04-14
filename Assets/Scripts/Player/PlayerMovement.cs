using JetBrains.Annotations;
using System;
using System.Buffers.Text;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[Serializable]
public struct PlayerMovementData
{
    public PlayerMovementData(float Speed, Vector3 Input, float Gravity, float YAdd)
    {
        this.Speed = Speed;
        this.Input = Input;
        this.Gravity = Gravity;
        this.YAdd = YAdd;
    }
    public float Speed;
    public Vector3 Input;
    public float Gravity;
    public float YAdd;
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

    [SerializeField] PlayerMovementData Data;

    [SerializeField] Vector3 targetWallRunDir;

    public float WallUpAdd;

    Vector3 Dir;

    public Vector3 WallJump;

    Vector3 MoveVector;

    public float WallExitAngle;

    float wallexittime;

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
                Dir = transform.forward * Data.Input.z + transform.right * Data.Input.x;
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

        cc.Move(MoveVector * Time.deltaTime);

        MoveVector = Vector3.zero;

        if (Data.Input != Vector3.zero||(IsWall))
        {
            OnMoveStarted?.Invoke();
        }
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
        }
        

        if (Data.Input == Vector3.zero&&!IsWall)
        {
            OnMoveStopped?.Invoke();
        }
    }

    public void Jump(float JumpPower)
    {
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
        
    }
}
