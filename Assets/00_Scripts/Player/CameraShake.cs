using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("흔들림의 세기")]
    public float YAmplitude;
    public float XAmplitude;

    [Header("흔들림의 속도")]
    public float YSpeed;
    public float XSpeed;

    [Header("기본위치")]
    public float BaseY;
    public float BaseX;

    [Header("흔들림 시간")]
    public float YTimer;
    public float XTimer;

    [Header("원래 위치로 돌아오는 속도")]
    public float BackSpeed;

    bool shaking;

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
        if (shaking)
        {
            shaking = false;
            YTimer = 0;
            XTimer = 0;
            StartCoroutine(BackCamera(BackSpeed));
        }
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
