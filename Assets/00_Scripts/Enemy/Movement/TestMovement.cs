using UnityEngine;
using System.Collections.Generic;

public class TestMovement : MovementAIBase
{
    public float OnMoreMoveTimer;
    //float OnMoreMoveTime = 0;
    public float ArriveDistance;
    Rigidbody rb;

    EnemyInfomation EnemyInfo;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnemyInfo = new EnemyInfomation(0);
    }

    public override void OnStart(EnemyInfomation enemyInfo)
    {
        EnemyInfo = new EnemyInfomation(enemyInfo.Speed);
        //if (OnMoreMoveTime < OnMoreMoveTimer)
        //{
        //    OnMoreMoveTime += Time.deltaTime;
        //    return;
        //}
        //else
        //{
        //    OnMoreMoveTime = 0;
            
        //}
        IsMoving = true;
        Debug.Log("이동 시작");
    }
    public override void OnMove()
    {
        //Quaternion targetrotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(WayPoints[CurrentWayPIndex].transform.position - transform.position), Time.deltaTime * 5f);
        //targetrotation.x = 0;
        //targetrotation.z = 0;
        //transform.rotation = targetrotation;


        Transform target = WayPoints[CurrentWayPIndex];

        Vector3 currentPos = rb.position;
        Vector3 targetPos = target.position;

        //targetPos.y = currentPos.y;

        Vector3 toTarget = targetPos - currentPos;
        float distance = toTarget.magnitude;

        if (distance <= ArriveDistance)
        {
            if (CurrentWayPIndex < WayPoints.Count - 1)
            {
                InvokeOnWayPoint();
                CurrentWayPIndex++;
            }
            else
            {
                FinalArrived = true;
            }
            //return;
        }

        Debug.Log("타겟" + targetPos);
        Vector3 dir = toTarget / distance;
        if (dir.sqrMagnitude > 0.001f)
        {
            Vector3 nextPos = currentPos + dir * EnemyInfo.Speed * Time.fixedDeltaTime;
            //Debug.Log(currentPos + "에서" + nextPos + "로 이동 방향은" + dir);
            rb.MovePosition(nextPos);

            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion nextRot = Quaternion.Slerp(rb.rotation, targetRot, 5 * Time.fixedDeltaTime);
            rb.MoveRotation(nextRot);
        }
    }
    public override void OnStop()
    {
        //OnMoreMoveTime = 0;
        IsMoving = false;
        Debug.Log("이동 정지");
    }
}
