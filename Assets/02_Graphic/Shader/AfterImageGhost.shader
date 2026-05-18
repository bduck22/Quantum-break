Shader "Hidden/Custom/FakeDashGhost"
{
    Properties
    {
        _Intensity ("Intensity", Range(0, 1)) = 0
        _Offset ("Offset", Range(0, 0.05)) = 0.01
        _Direction ("Direction", Vector) = (1, 0, 0, 0)
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _FadePower ("Fade Power", Range(0.1, 5)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Overlay"
        }

        Pass
        {
            Name "FakeDashGhost"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);

            float _Intensity;
            float _Offset;
            float4 _Direction;
            float4 _Tint;
            float _FadePower;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                half4 currentColor = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                );

                float2 dir = _Direction.xy;
                float len = length(dir);

                if (len > 0.0001)
                {
                    dir /= len;
                }
                else
                {
                    dir = float2(1, 0);
                }

                // 대시 방향의 반대쪽으로 화면 샘플을 밀어서 잔상처럼 보이게 함
                float2 ghostDir = -dir;

                half4 ghost1 = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + ghostDir * _Offset * 1.0
                );

                half4 ghost2 = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + ghostDir * _Offset * 2.0
                );

                half4 ghost3 = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv + ghostDir * _Offset * 3.0
                );

                ghost1.rgb *= _Tint.rgb;
                ghost2.rgb *= _Tint.rgb;
                ghost3.rgb *= _Tint.rgb;

                half4 ghostColor = currentColor;

                ghostColor.rgb += ghost1.rgb * 0.25;
                ghostColor.rgb += ghost2.rgb * 0.15;
                ghostColor.rgb += ghost3.rgb * 0.08;

                ghostColor.rgb /= 1.48;

                float strength = pow(saturate(_Intensity), _FadePower);

                half4 finalColor = lerp(currentColor, ghostColor, strength);
                finalColor.a = currentColor.a;

                return finalColor;
            }

            ENDHLSL
        }
    }
}