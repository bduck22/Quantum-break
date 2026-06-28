Shader "Hidden/IWantGoHome/PlayerHitAfterimage"
{
    Properties
    {
        _Mode ("Mode", Float) = 0
        _Progress ("Progress", Range(0, 1)) = 1
        _ManualTime ("Manual Time", Float) = 0
        _Aspect ("Aspect", Float) = 1.777777
        _SnapshotTex ("Snapshot Texture", 2D) = "black" {}

        _AfterimageIntensity ("Afterimage Intensity", Range(0, 2)) = 1.35
        _AfterimageOffset ("Afterimage Offset", Range(0, 0.02)) = 0.0018
        _RGBSplit ("RGB Split", Range(0, 0.02)) = 0.0014
        _ZoomAmount ("Zoom Amount", Range(0, 0.01)) = 0.00010
        _CenterGlowIntensity ("Center Glow Intensity", Range(0, 2)) = 0.0
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.02
        _SnapshotFlipX ("Snapshot Flip X", Float) = 0
        _SnapshotFlipY ("Snapshot Flip Y", Float) = 1
        _Pivot ("Pivot", Vector) = (0.5, 0.5, 0, 0)
        _RandomShift ("Random Shift", Vector) = (0, 0, 0, 0)
        _SecondarySpread ("Secondary Spread", Float) = 0.00045
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "Player Hit Afterimage Aligned"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_SnapshotTex);
            SAMPLER(sampler_SnapshotTex);

            float _Mode;
            float _Progress;
            float _ManualTime;
            float _Aspect;
            float _AfterimageIntensity;
            float _AfterimageOffset;
            float _RGBSplit;
            float _ZoomAmount;
            float _CenterGlowIntensity;
            float _VignetteIntensity;
            float _SnapshotFlipX;
            float _SnapshotFlipY;
            float4 _Pivot;
            float4 _RandomShift;
            float _SecondarySpread;

            half4 SampleScene(float2 uv)
            {
                uv = saturate(uv);
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, UnityStereoTransformScreenSpaceTex(uv));
            }

            float2 OrientSnapshotUV(float2 uv)
            {
                if (_SnapshotFlipX > 0.5) uv.x = 1.0 - uv.x;
                if (_SnapshotFlipY > 0.5) uv.y = 1.0 - uv.y;
                return saturate(uv);
            }

            half4 SampleSnapshot(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_SnapshotTex, sampler_SnapshotTex, OrientSnapshotUV(uv));
            }

            float2 ZoomAroundPivot(float2 uv, float2 pivot, float zoom)
            {
                return pivot + (uv - pivot) / max(0.001, 1.0 + zoom);
            }

            float Hash12(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float3 SampleSnapshotRGB(float2 uv, float split)
            {
                float3 c;
                c.r = SampleSnapshot(uv + float2(split, 0.0)).r;
                c.g = SampleSnapshot(uv).g;
                c.b = SampleSnapshot(uv - float2(split, 0.0)).b;
                return c;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float mode = floor(_Mode + 0.5);
                half4 scene = SampleScene(uv);

                if (mode < 0.5)
                {
                    return scene;
                }

                float p = saturate(_Progress);
                float fade = 1.0 - smoothstep(0.10, 1.0, p);
                fade = pow(saturate(fade), 0.90);
                float impact = 1.0 - smoothstep(0.0, 0.14, p);

                float2 centered = uv * 2.0 - 1.0;
                centered.x *= _Aspect;
                float dist = length(centered);
                float vignette = smoothstep(0.52, 1.36, dist) * _VignetteIntensity * fade;

                // Keep the afterimage close, but choose its side/direction randomly per hit.
                float offset = _AfterimageOffset * lerp(1.0, 0.20, smoothstep(0.0, 1.0, p));
                float split = _RGBSplit * lerp(1.0, 0.22, smoothstep(0.0, 1.0, p));
                float zoom = _ZoomAmount * fade;

                float wobble = sin(_ManualTime * 18.0 + uv.y * 8.0) * 0.00008 * impact;
                float2 pivot = _Pivot.xy;
                float2 randomShift = _RandomShift.xy;
                float dirLen = max(length(randomShift), 0.000001);
                float2 dir = randomShift / dirLen;
                float2 perp = float2(-dir.y, dir.x);
                float spread = _SecondarySpread;

                float2 baseUv = ZoomAroundPivot(uv + randomShift + float2(wobble, 0.0), pivot, zoom);

                float3 alignedSnapshot = SampleSnapshot(baseUv).rgb;
                float3 rgbSnapshot = SampleSnapshotRGB(baseUv, split);
                float3 closeEchoA = SampleSnapshot(ZoomAroundPivot(uv + randomShift * 0.82 + perp * spread, pivot, zoom)).rgb;
                float3 closeEchoB = SampleSnapshot(ZoomAroundPivot(uv + randomShift * 0.56 - perp * spread * 0.75, pivot, zoom)).rgb;
                float3 closeEchoC = SampleSnapshot(ZoomAroundPivot(uv + randomShift * 0.34 + dir * offset * 0.65, pivot, zoom)).rgb;

                float3 ghost = alignedSnapshot * 0.68;
                ghost += rgbSnapshot * 0.16;
                ghost += closeEchoA * 0.07;
                ghost += closeEchoB * 0.055;
                ghost += closeEchoC * 0.04;

                float lum = dot(ghost, float3(0.2126, 0.7152, 0.0722));
                ghost += lum * 0.035;

                float grain = Hash12(uv * float2(1417.0, 803.0) + floor(_ManualTime * 32.0));
                ghost += (grain - 0.5) * 0.0020 * impact;

                float strength = saturate(_AfterimageIntensity * fade);
                float3 col = scene.rgb;

                col = lerp(col, ghost, strength * 0.43);
                col += ghost * strength * 0.08;

                float3 fringe = SampleSnapshotRGB(baseUv, split * 1.10);
                col = lerp(col, fringe, strength * 0.045);

                col *= (1.0 - vignette);

                return half4(saturate(col), 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
