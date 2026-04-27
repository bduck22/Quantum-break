using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float YAmplitude;
    public float XAmplitude;

    public float YSpeed;
    public float XSpeed;

    public float BaseY;
    public float BaseX;

    public float YTimer;
    public float XTimer;

    public float BackSpeed;

    bool shaking;

    public float StepYT;
    public float StepXT;

    public float StepSpeed;

    void Start()
    {
        BaseY = transform.localPosition.y;
    }

    private void Update()
    {
        if (shaking)
        {
            YTimer += Time.deltaTime * (YSpeed);
            XTimer += Time.deltaTime * (XSpeed);

            float Ybob = Mathf.Sin(YTimer) * (YAmplitude);
            float Xbob = Mathf.Sin(XTimer) * (XAmplitude);

            transform.localPosition = new Vector3(BaseX + Xbob, BaseY + Ybob, 0);
        }
    }

    public void Shake()
    {
        shaking = true;
    }

    public void StopShake()
    {
        shaking = false;
        YTimer = 0;
        XTimer = 0;
        StartCoroutine(BackCamera(BackSpeed));
        //transform.localPosition = new Vector3(BaseX, BaseY, 0);
    }


    IEnumerator BackCamera(float speed)
    {
        while (transform.localPosition.y != BaseY || transform.localPosition.x != BaseX)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(BaseX, BaseY, 0), Time.deltaTime*speed);
            yield return null;
        }
    }
}
