using System.Buffers.Text;
using System.Collections;
using UnityEngine;

public class FOV : MonoBehaviour
{
    Camera cam;

    public float DefaultFOV;

    public float CurrentFOV;

    public float FOVRange;

    public float FOVUpSpeed;

    public float FOVBackSpeed;

    private void Start()
    {
        cam = Camera.main;

        CurrentFOV = DefaultFOV;

        cam.fieldOfView = CurrentFOV;
    }

    private void Update()
    {
        if(cam.fieldOfView != CurrentFOV)
        {
            cam.fieldOfView = CurrentFOV;
        }
    }

    public void FOVUp()
    {
        if(CurrentFOV <= (DefaultFOV + FOVRange))
        {
            CurrentFOV+= Time.deltaTime * FOVUpSpeed;
        }
    }

    public void BackFOV()
    {
        StartCoroutine(backFOV());
    }

    IEnumerator backFOV()
    {
        while (CurrentFOV > DefaultFOV)
        {
            CurrentFOV -= Time.deltaTime * FOVBackSpeed;
            
            yield return new WaitForEndOfFrame();
        }
        CurrentFOV = DefaultFOV;
    }
}
