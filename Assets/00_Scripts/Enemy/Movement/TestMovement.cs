using UnityEngine;
using System.Collections.Generic;

public class TestMovement : MovementAIBase
{
    public float OnMoreMoveTimer;
    //float OnMoreMoveTime = 0;
    public float ArriveDistance;
    Rigidbody rb;

    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Speed = 0;
    }

    public override void OnStart()
    {
        IsMoving = true;
    }

    public override void SpeedRefresh(float Speed)
    {
        this.Speed = Speed;
    }

    public override void OnMove()
    {
        Transform target = WayPoints[CurrentWayPIndex];

        Vector3 currentPos = rb.position;

        RaycastHit hit;
        Vector3 targetPos;
        if (Physics.Raycast(target.position, Vector3.down, out hit, 100, groundLayer))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = target.position;
        }

        Vector3 toTarget = targetPos - currentPos;
        float distance = toTarget.magnitude;

        if (distance <= ArriveDistance)
        {
            if (CurrentWayPIndex < WayPoints.Length - 1)
            {
                InvokeOnWayPoint();
                CurrentWayPIndex++;
            }
            else
            {
                FinalArrived = true;
            }
        }

        Vector3 dir = toTarget / distance;
        if (dir.sqrMagnitude > 0.001f)
        {
            Vector3 nextPos = currentPos + dir * Speed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);

            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion nextRot = Quaternion.Slerp(rb.rotation, targetRot, 10 * Time.fixedDeltaTime);
            rb.MoveRotation(nextRot);
        }
    }
    public override void OnStop()
    {
        IsMoving = false;
    }
}
