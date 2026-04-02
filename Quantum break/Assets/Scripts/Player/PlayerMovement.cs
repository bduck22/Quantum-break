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

    float holdtimer;

    public float HoldTime=0;

    public event Action OnMoveStarted;

    public event Action OnMoveStopped;

    [SerializeField] PlayerMovementData Data;

    private void Start()
    {
        cc = GetComponent<CharacterController>();

        Data = new PlayerMovementData(10, Vector3.zero, 20, 15);
    }

    private void Update()
    {
        if (isHoldingJump)
        {
            holdtimer += Time.deltaTime;
        }
        else
        {
            holdtimer = 0;
        }

        Vector3 Dir = CameraTransform.forward * Data.Input.z + CameraTransform.right * Data.Input.x;

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
        YVeolocity -= Data.Gravity * Time.deltaTime;
        if (YVeolocity < 0 || (!isHoldingJump && holdtimer <= HoldTime))
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
    }

    public void Move(PlayerMovementData Data)
    {
        Data.Input = new Vector3(Data.Input.x, 0, Data.Input.y);
        if (this.Data.Input != Data.Input)
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
        YVeolocity += JumpPower;
    }

    public void Dash()
    {
        
    }
}
