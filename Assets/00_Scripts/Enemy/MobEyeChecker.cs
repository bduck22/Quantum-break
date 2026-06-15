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
    [HideInInspector]
    int layer;

    private void Start()
    {
        layer = LayerMask.GetMask("PlayerMapCol") | LayerMask.GetMask("Map");
    }

    public void Init(PlayerController Player)
    {
        LockOn = false;
        this.Target = Player.transform;
        lockontime = 0;
    }

    public bool CheckTargetInEye()
    {
        Vector3 targetPosition = Target.position + new Vector3(0, Head.localPosition.y, 0);
        Vector3 dir = targetPosition - Head.position;

        

        if (Target.gameObject.layer != 9)
        {
            LockOn = true;
            lockontime = 0;
            return true;
        }

        if (Vector3.Magnitude(dir) <= MinCheckDistance)
        {
            LockOn = true;
            lockontime = 0;
            return true;
        }

        if (Vector3.Magnitude(dir) <= MaxCheckDistance)
        {
            if (LockOn)
            {
                if(Mathf.Abs(dir.y) > 15)
                {
                    if (lockontime >= StopToMoveTimer)
                    {
                        lockontime = 0;
                        LockOn = false;
                    }
                    else
                    {
                        lockontime += Time.deltaTime;
                    }
                    return false;
                }

                if (CheckingPlayerInEye(dir)) //플레이어가 있음
                {
                    lockontime = 0;
                    return true;
                }
                else //플레이어가 숨음
                {
                    if (lockontime >= StopToMoveTimer)
                    {
                        lockontime = 0;
                        LockOn = false;
                    }
                    else
                    {
                        lockontime += Time.deltaTime;
                    }
                    return false;
                }
            }
            else
            {
                if (CheckingPlayerInEye(dir))
                {
                    Vector3 forward = transform.forward;
                    Vector3 dirToTarget = dir;

                    float angle = Vector3.Angle(forward, dirToTarget);
                    if (angle <= EyeAngle * 0.5f&&Mathf.Abs(dir.y)<15)
                    {
                        LockOn = true;
                        lockontime = 0;
                        return true;
                    }
                }
            }
        }

        LockOn = false;
        return false;
    }

    public bool CheckingPlayerInEye(Vector3 dir)
    {
        RaycastHit hit;
        if(Physics.Raycast(Head.position, (dir).normalized, out hit ,MaxCheckDistance, layer))
        {
            if(hit.transform.gameObject.layer == 9)
            {
                return true;
            }
        }
        return false;
    }
}
