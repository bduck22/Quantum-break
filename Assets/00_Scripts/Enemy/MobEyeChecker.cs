using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MobEyeChecker : MonoBehaviour
{
    public Transform Head;
    [HideInInspector]
    public Transform Target;

    [Header("최대 인식 범위")]
    public float MaxCheckDistance;

    [Header("최소 인식 범위")]
    public float MinCheckDistance;

    [Header("처음 인식 각도")]
    public float EyeAngle;

    [Header("타겟팅 온")]
    public bool LockOn;

    [Header("이동전환 속도")]
    public float StopToMoveTimer;
    [HideInInspector]
    public float lockontime;

    public void Init(PlayerController Player)
    {
        this.Target = Player.transform;
    }

    public bool CheckTargetInEye()
    {
        Vector3 targetPosition = Target.position + new Vector3(0, Head.localPosition.y, 0);
        Vector3 dir = targetPosition - Head.position;

        if (Target.gameObject.layer != 9)
        {
            LockOn = true;
            return true;
        }

        //if (LockOn)
        //{
        //    if (Vector3.Magnitude(dir) >= MaxCheckDistance)
        //    {
        //        LockOn = false;
        //        return false;
        //    }

        //    if (!CheckingPlayerInEye(dir))
        //    {
        //        LockOn = false;
        //        return false;
        //        //LockOn = false;
        //        //return false;
        //    }


        //        return true;
        //}


        if (Vector3.Magnitude(dir) <= MinCheckDistance)
        {
            LockOn = true;
            return true;
        }

        if (Vector3.Magnitude(dir) <= MaxCheckDistance)
        {
            if (LockOn)
            {
                if (CheckingPlayerInEye(dir)) //플레이어가 있음
                {
                    lockontime = 0;
                    return true;
                }
                else //플레이어가 숨음
                {
                    if(lockontime >= StopToMoveTimer)
                    {
                        lockontime = 0;
                        LockOn = false;
                        return false;
                    }
                    else
                    {
                        lockontime += Time.deltaTime;
                    }
                }
            }
            else
            {
                if (CheckingPlayerInEye(dir))
                {
                    Vector3 forward = transform.forward;
                    Vector3 dirToTarget = dir;

                    float angle = Vector3.Angle(forward, dirToTarget);
                    if (angle <= EyeAngle * 0.5f)
                    {
                        LockOn = true;
                        return true;
                    }
                }
            }
        }

        //if (CheckingPlayerInEye(dir))
        //{
        //    if (LockOn)
        //    {
        //        return true;
        //    }
        //}
        //else
        //{
        //    LockOn = false;
        //}
        return false;
    }

    public bool CheckingPlayerInEye(Vector3 dir)
    {
        int layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Map");
        RaycastHit hit;
        if(Physics.Raycast(Head.position, (dir).normalized, out hit ,MaxCheckDistance, layer))
        {
            if(hit.transform.gameObject.layer == 7)
            {
                //Debug.Log("플레이어 있음" + hit.transform.gameObject.layer);
                return true;
            }
        }
        //Debug.Log("플레이어 없음" + hit.transform.name);
        return false;
    }
}
