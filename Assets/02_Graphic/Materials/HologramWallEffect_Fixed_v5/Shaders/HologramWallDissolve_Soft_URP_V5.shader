Shader "Custom/HologramWall/TransparentDissolve_Soft_URP_V5"
{
    Properties
    {
        [Header(Soft Transparent Hologram)]
        _TintColor ("Face Tint Color", Color) = (0.04, 0.70, 1.00, 0.28)
        _LineColor ("Line/Rim Color", Color) = (0.16, 0.95, 1.00, 1.00)
        _FaceAlpha ("Face Alpha", Range(0, 1)) = 0.24
        _OverallBrightness ("Overall Brightness", Range(0, 2)) = 0.72
        _MaxOutputBrightness ("Max Output Brightness", Range(0.25, 2)) = 1.08

        [Header(Sparse Horizontal Lines)]
        _LineSpacing ("Line Spacing World Units", Range(0.05, 3.0)) = 0.82
        _LineWidth ("Line Width", Range(0.002, 0.12)) = 0.018
        _LineBrightness ("Line Brightness", Range(0, 3)) = 0.75
        _LineScrollSpeed ("Line Scroll Speed", Range(-5, 5)) = -0.16

        [Header(Top Dot Grid)]
        _GridSpacing ("Grid Spacing World Units", Range(0.05, 3.0)) = 0.75
        _GridWidth ("Grid Width", Range(0.002, 0.08)) = 0.008
        _GridBrightness ("Grid Brightness", Range(0, 1.5)) = 0.025

        [Header(Rim)]
        _RimPower ("Rim Power", Range(0.2, 8)) = 2.5
        _RimBrightness ("Rim Brightness", Range(0, 3)) = 0.85

        [Header(Dissolve)]
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0.001, 0.25)) = 0.055
        _DissolveEdgeBrightness ("Dissolve Edge Brightness", Range(0, 5)) = 1.25
        _NoiseScale ("Noise Scale", Range(0.1, 20)) = 2.7
        _VerticalDissolveBias ("Vertical Dissolve Bias", Range(-1.5, 1.5)) = 0.10

        [Header(Subtle Motion)]
        _SurfaceNoiseStrength ("Surface Noise Strength", Range(0, 0.4)) = 0.055
        _FlickerStrength ("Flicker Strength", Range(0, 0.5)) = 0.035
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "SoftHologramUnlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _TintColor;
                float4 _LineColor;
                float _FaceAlpha;
                float _OverallBrightness;
                float _MaxOutputBrightness;

                float _LineSpacing;
                float _LineWidth;
                float _LineBrightness;
                float _LineScrollSpeed;

                float _GridSpacing;
                float _GridWidth;
                float _GridBrightness;

                float _RimPower;
                float _RimBrightness;

                float _DissolveAmount;
                float _DissolveEdgeWidth;
                float _DissolveEdgeBrightness;
                float _NoiseScale;
                float _VerticalDissolveBias;

                float _SurfaceNoiseStrength;
                float _FlickerStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
            };

            float Hash31(float3 p)
            {
                p = frac(p * 0.1031);
                p += dot(p, p.yzx + 33.33);
                return frac((p.x + p.y) * p.z);
            }

            float ValueNoise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                float n000 = Hash31(i + float3(0.0, 0.0, 0.0));
                float n100 = Hash31(i + float3(1.0, 0.0, 0.0));
                float n010 = Hash31(i + float3(0.0, 1.0, 0.0));
                float n110 = Hash31(i + float3(1.0, 1.0, 0.0));
                float n001 = Hash31(i + float3(0.0, 0.0, 1.0));
                float n101 = Hash31(i + float3(1.0, 0.0, 1.0));
                float n011 = Hash31(i + float3(0.0, 1.0, 1.0));
                float n111 = Hash31(i + float3(1.0, 1.0, 1.0));

                float nx00 = lerp(n000, n100, f.x);
                float nx10 = lerp(n010, n110, f.x);
                float nx01 = lerp(n001, n101, f.x);
                float nx11 = lerp(n011, n111, f.x);
                float nxy0 = lerp(nx00, nx10, f.y);
                float nxy1 = lerp(nx01, nx11, f.y);
                return lerp(nxy0, nxy1, f.z);
            }

            float SoftBand(float coord, float spacing, float width)
            {
                float safeSpacing = max(spacing, 0.0001);
                float repeatValue = coord / safeSpacing;
                float distanceToCenter = abs(frac(repeatValue) - 0.5);
                return 1.0 - smoothstep(width, width + 0.018, distanceToCenter);
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.positionOS = input.positionOS.xyz;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = input.uv;
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(input.positionWS));

                float baseNoise = ValueNoise(input.positionWS * _NoiseScale + float3(0.0, _Time.y * 0.12, 0.0));
                float wideNoise = ValueNoise(input.positionWS * (_NoiseScale * 0.45) + float3(_Time.y * 0.035, 0.0, _Time.y * 0.02));
                float dissolveMask = saturate(lerp(baseNoise, wideNoise, 0.38) + input.positionOS.y * _VerticalDissolveBias);

                float dissolveCut = dissolveMask - _DissolveAmount;
                clip(dissolveCut + 0.018);

                float dissolveEdge = 1.0 - smoothstep(0.0, max(_DissolveEdgeWidth, 0.0001), abs(dissolveCut));

                float horizontalBand = SoftBand(input.positionWS.y + _Time.y * _LineScrollSpeed, _LineSpacing, _LineWidth);

                float topFaceMask = smoothstep(0.55, 0.95, normalWS.y);
                float gridX = SoftBand(input.positionWS.x, _GridSpacing, _GridWidth);
                float gridZ = SoftBand(input.positionWS.z, _GridSpacing, _GridWidth);
                float topGrid = gridX * gridZ * topFaceMask;

                float rim = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _RimPower);

                float flickerWave = sin(_Time.y * 19.7) * 0.5 + sin(_Time.y * 37.1) * 0.5;
                float flicker = 1.0 + flickerWave * _FlickerStrength;

                float surfaceNoise = 1.0 + (baseNoise - 0.5) * _SurfaceNoiseStrength;

                float3 faceColor = _TintColor.rgb * 0.55 * surfaceNoise;
                float3 lineColor = _LineColor.rgb * horizontalBand * _LineBrightness;
                float3 gridColor = _LineColor.rgb * topGrid * _GridBrightness;
                float3 rimColor = _LineColor.rgb * rim * _RimBrightness;
                float3 edgeColor = _LineColor.rgb * dissolveEdge * _DissolveEdgeBrightness * smoothstep(0.005, 0.35, _DissolveAmount);

                float3 finalColor = (faceColor + lineColor + gridColor + rimColor + edgeColor) * _OverallBrightness * flicker;
                finalColor = min(finalColor, _MaxOutputBrightness.xxx);

                float alpha = _FaceAlpha * _TintColor.a;
                alpha += horizontalBand * 0.085;
                alpha += rim * 0.12;
                alpha += topGrid * 0.03;
                alpha += dissolveEdge * 0.20 * smoothstep(0.005, 0.35, _DissolveAmount);
                alpha = saturate(alpha);

                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
