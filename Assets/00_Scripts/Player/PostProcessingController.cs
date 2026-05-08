using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    Volume volume;


    [Header("렌즈효과")]
    LensDistortion lensDistortion;
    public float LensUpSpeed;
    public float LensDownSpeed;
    public bool lensoff;

    [Header("모션블러")]
    MotionBlur motionBlur;
    public float MotionDownSpeed;

    [Header("비그네트")]
    Vignette vignette;
    public float VignetteUpSpeed;
    public float VignetteDownSpeed;
    public bool vigoff;

    [Header("흐림효과")]
    DepthOfField depthOfField;

    [Header("화면 색상 필터")]
    ColorAdjustments colorAdjustments;
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out motionBlur);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out depthOfField);
        volume.profile.TryGet(out colorAdjustments);
    }

    public void DashFilterOn()
    {
        vignette.intensity.value = 0;
        lensDistortion.intensity.value = 0;
        lensDistortion.active = true;
        lensoff = false;
        vignette.active = true;
        depthOfField.active = true;
    }

    public void DashFilterOff()
    {
        lensDistortion.intensity.value = 0.4f;
        motionBlur.intensity.value = 0.4f;
        motionBlur.clamp.value = 0.15f;
        lensoff = true;
        vigoff = true;
    }

    private void Update()
    {
        if (lensDistortion.active)
        {
            if (lensoff)
            {
                lensDistortion.intensity.value -= Time.unscaledDeltaTime*LensDownSpeed;
                if (lensDistortion.intensity.value <= 0)
                {
                    lensDistortion.active = false;
                    lensoff = false;
                }
            }
            else if(lensDistortion.intensity.value <= 0.4f)
            {
                lensDistortion.intensity.value += Time.unscaledDeltaTime*LensUpSpeed;
            }
        }


        if(motionBlur.intensity.value > 0.3f)
        {
            motionBlur.intensity.value -= Time.unscaledDeltaTime*MotionDownSpeed;
        }
        else if(motionBlur.intensity.value != 0.3f)
        { 
            motionBlur.intensity.value = 0.3f;
        }


        if(motionBlur.clamp.value > 0.03f)
        {
            motionBlur.clamp.value -= Time.unscaledDeltaTime * MotionDownSpeed;
        }
        else if(motionBlur.clamp.value != 0.03f)
        {
            motionBlur.clamp.value = 0.03f;
        }

        if (vignette.active)
        {
            if (vigoff)
            {
                vignette.intensity.value -= Time.unscaledDeltaTime*VignetteDownSpeed;
                if(vignette.intensity.value <= 0)
                {
                    colorAdjustments.colorFilter.overrideState = false;
                    depthOfField.active = false;
                    vignette.active = false;
                    vigoff = false;
                }
            }
            else if(vignette.intensity.value < 0.5f)
            {
                vignette.intensity.value += Time.unscaledDeltaTime*VignetteUpSpeed;
            }
            else if (!colorAdjustments.colorFilter.overrideState)
            {
                colorAdjustments.colorFilter.overrideState = true;
            }
        }
    }
}
