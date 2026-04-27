using System.Buffers.Text;
using System.Collections;
using UnityEngine;

public class FOV : MonoBehaviour
{
    Camera cam;

    public float DefaultFOV;

    public float FOVRange;

    public float FOVDash;

    public float FOVUpSpeed;

    public float FOVBackSpeed;

    public float TargetFOV;

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
        if (TargetFOV < DefaultFOV + FOVRange)
        {
            TargetFOV = DefaultFOV + FOVRange;
        }
    }

    public void BackFOV()
    {
        TargetFOV = DefaultFOV;
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
