using UnityEngine;

public class CameraHighlightLineDraw : MonoBehaviour
{
    public Material Highlight;

    public float MaxIntensity;

    public float Intensity;

    public float Speed;

    public float MinInner;
    public float MaxInner;

    bool IsOn;

    void Update()
    {
        //바깥으로 쏘는 코드
        if (IsOn)
        {
            if (Intensity < MaxIntensity)
            {
                Intensity += Time.unscaledDeltaTime * Speed;
                Highlight.SetFloat("_Intensity", Intensity);
                Highlight.SetFloat("_InnerMask", MinInner + ((Intensity / MaxIntensity) * (MaxInner - MinInner)));
            }
            else if (Intensity != MaxIntensity)
            {
                Intensity = MaxIntensity;
                Highlight.SetFloat("_Intensity", Intensity);
                Highlight.SetFloat("_InnerMask", MaxInner);
                IsOn = false;
            }
        }
        else
        {
            if (Intensity > 0f)
            {
                Intensity -= Time.unscaledDeltaTime * Speed;
                Highlight.SetFloat("_Intensity", Intensity);
            }
            else if (Intensity != 0)
            {
                Intensity = 0;
                Highlight.SetFloat("_Intensity", Intensity);
                Highlight.SetFloat("_InnerMask", MinInner);
            }
        }


        //중앙으로 모이는 코드
        //if (IsOn)
        //{
        //    if (Intensity < MaxIntensity)
        //    {
        //        Intensity += Time.unscaledDeltaTime * Speed;
        //        Highlight.SetFloat("_Intensity", Intensity);
        //        Highlight.SetFloat("_InnerMask", MinInner + ((1 - (Intensity / MaxIntensity)) * (MaxInner - MinInner)));
        //    }
        //    else if (Intensity != MaxIntensity)
        //    {
        //        Intensity = MaxIntensity;
        //        Highlight.SetFloat("_Intensity", Intensity);
        //        Highlight.SetFloat("_InnerMask", MinInner);
        //        IsOn = false;
        //    }
        //}
        //else
        //{
        //    if (Intensity > 0f)
        //    {
        //        Intensity -= Time.unscaledDeltaTime * Speed;
        //        Highlight.SetFloat("_Intensity", Intensity);
        //        Highlight.SetFloat("_InnerMask", MinInner + ((1 - (Intensity / MaxIntensity)) * (MaxInner - MinInner)));
        //    }
        //    else if (Intensity != 0)
        //    {
        //        Intensity = 0;
        //        Highlight.SetFloat("_Intensity", Intensity);
        //        Highlight.SetFloat("_InnerMask", MaxInner);
        //    }
        //}
    }

    public void OnDash()
    {
        IsOn = true;
    }
}
