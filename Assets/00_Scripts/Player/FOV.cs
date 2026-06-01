using System.Buffers.Text;
using System.Collections;
using UnityEngine;

public class FOV : MonoBehaviour
{
    Camera cam;

    [Header("기본 FOV")]
    public float DefaultFOV;

    [Header("달릴 때 추가되는 FOV 범위")]
    public float FOVRange;

    [Header("대쉬할 때 추가되는 FOV")]
    public float FOVDash;

    [Header("FOV Up 속도")]
    public float FOVUpSpeed;

    [Header("FOV Back 속도")]
    public float FOVBackSpeed;

    [Header("현재 목표 FOV")]
    public float TargetFOV;

    [Header("맞았을 때 줄어드는 FOV")]
    public float HitFOV;

    [Header("걷는 FOV 버퍼")]
    [SerializeField] private bool RunBuffer;
    private void Start()
    {
        cam = Camera.main;

        TargetFOV = DefaultFOV;

        cam.fieldOfView = TargetFOV;
    }

    private void Update()
    {
        if(cam.fieldOfView != TargetFOV)
        {
            if (cam.fieldOfView > TargetFOV)
            {
                cam.fieldOfView -= FOVBackSpeed * Time.unscaledDeltaTime;
                if(cam.fieldOfView - TargetFOV <= 0)
                {
                    cam.fieldOfView = TargetFOV;
                }
            }
            else if (cam.fieldOfView < TargetFOV) {
                cam.fieldOfView += FOVUpSpeed * Time.unscaledDeltaTime;
                if (cam.fieldOfView - TargetFOV >= 0)
                {
                    cam.fieldOfView = TargetFOV;
                }
            }
        }
    }

    public void FOVUp()
    {
        if (TargetFOV == DefaultFOV || TargetFOV == (DefaultFOV + FOVRange))
        {
            TargetFOV = DefaultFOV + FOVRange;
        }
        //else
        //{
        //    RunBuffer = true;
        //}
    }

    public void HitedFOV()
    {
        TargetFOV = DefaultFOV - HitFOV;
    }

    public void HitBackFOV()
    {
        TargetFOV = DefaultFOV;
    }

    public void BackFOV()
    {
        if(TargetFOV >= DefaultFOV)
        {
            TargetFOV = DefaultFOV;
        }
    }

    public void DashFOV()
    {
        cam.fieldOfView = DefaultFOV + FOVDash;
    }

    public void DashingFOV()
    {
        TargetFOV = DefaultFOV + FOVDash-14;
    }
}
