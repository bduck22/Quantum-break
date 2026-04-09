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

    private void Start()
    {
        cc = GetComponent<CharacterController>();

        Data = new PlayerMovementData(10, Vector3.zero, 20, 15);
    }

    private void Update()
    {
        Vector3 Dir;
        if (IsWall)
        {
            Dir = targetWallRunDir + new Vector3(0,WallUpAdd,0);
        }
        else
        {
            Dir = CameraTransform.forward * Data.Input.z + CameraTransform.right * Data.Input.x;
        }

        Dir.y = 0;

        gravity();

        Vector3 Move = Dir * Data.Speed;
        Move.y = YVeolocity;

        cc.Move(Move * Time.deltaTime);

        if (Data.Input != Vector3.zero)
        {
            OnMoveStarted?.Invoke();
        }
    }

    private void gravity() {
        if (IsWall)
        {
            float viewAlignment = Vector3.Dot(CameraTransform.forward, targetWallRunDir);

            float minDot = Mathf.Cos(50f * Mathf.Deg2Rad);

            Debug.Log($"View Alignment: {viewAlignment}, Min Dot: {minDot}");

            if (viewAlignment >= minDot)
            {
                YVeolocity = 0;
                return;
            }
        }
        YVeolocity -= Data.Gravity * Time.deltaTime;
        if (YVeolocity < 0 || (!isHoldingJump))
        {
            if (cc.isGrounded)
            {
                YVeolocity = -1f;
            }
            else
            {
                YVeolocity -= Data.YAdd * Time.deltaTime;
            }
        }
        YVeolocity = Mathf.Clamp(YVeolocity, MinGravity, MaxGravity);
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
    }

    public void Move(PlayerMovementData Data)
    {
        Data.Input = new Vector3(Data.Input.x, 0, Data.Input.y);
        if (this.Data.Input != Data.Input || this.Data.Gravity != Data.Gravity)
        {
            this.Data = Data;
        }
        

        if (Data.Input == Vector3.zero)
        {
            OnMoveStopped?.Invoke();
        }
    }

    public void Jump(float JumpPower)
    {
        IsWall = false;
        YVeolocity += JumpPower;
    }

    public void Dash()
    {
        
    }
}
