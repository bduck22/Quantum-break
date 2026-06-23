Shader "Hidden/IWantGoHome/TVStarGlitchV10"
{
    Properties
    {
        _Mode ("Mode", Float) = 0
        _Progress ("Progress", Range(0, 1)) = 0
        _ManualTime ("Manual Time", Float) = 0
        _Aspect ("Aspect", Float) = 1.777777
        _HoldSeed ("Hold Seed", Float) = 31.73

        _SceneGlitchIntensity ("Scene Glitch Intensity", Range(0, 3)) = 1.28
        _RGBSplit ("RGB Split", Range(0, 0.16)) = 0.038
        _HorizontalTear ("Horizontal Tear", Range(0, 0.30)) = 0.145
        _WaveDistortion ("Wave Distortion", Range(0, 0.12)) = 0.020
        _FineNoise ("Fine Noise", Range(0, 0.08)) = 0.007

        _HoldBurstIntensity ("Held Burst Intensity", Range(0, 3)) = 1.00
        _HoldLineDensity ("Held Line Density", Range(0.1, 5)) = 2.60
        _HoldVerticalSpikeIntensity ("Held Vertical Spikes", Range(0, 1)) = 0.04

        _StarEdge ("Star Edge Softness", Range(0.0005, 0.04)) = 0.0030
        _StarSharpness ("Star Sharpness", Range(1, 48)) = 22
        _StarHorizontalReach ("Star Horizontal Reach", Range(0.5, 8)) = 2.85
        _StarVerticalReach ("Star Vertical Reach", Range(0.1, 3)) = 0.52
        _StarHorizontalThickness ("Horizontal Thickness", Range(0.001, 0.40)) = 0.090
        _StarVerticalThickness ("Vertical Thickness", Range(0.001, 0.40)) = 0.030
        _StarTipWidth ("Star Tip Width", Range(0.0001, 0.02)) = 0.0010
        _StarIntensity ("Star Intensity", Range(0, 5)) = 2.25
        _StarGlowIntensity ("Star Glow Intensity", Range(0, 3)) = 0.16

        _FlashIntensity ("Power On Flash", Range(0, 8)) = 3.65
        _AfterimageIntensity ("Afterimage Intensity", Range(0, 2)) = 1.35
        _AfterimageOffset ("Afterimage Offset", Range(0, 0.20)) = 0.070
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "TV Star Glitch V10"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Mode, _Progress, _ManualTime, _Aspect, _HoldSeed;
            float _SceneGlitchIntensity, _RGBSplit, _HorizontalTear, _WaveDistortion, _FineNoise;
            float _HoldBurstIntensity, _HoldLineDensity, _HoldVerticalSpikeIntensity;
            float _StarEdge, _StarSharpness, _StarHorizontalReach, _StarVerticalReach, _StarHorizontalThickness, _StarVerticalThickness, _StarTipWidth, _StarIntensity, _StarGlowIntensity;
            float _FlashIntensity, _AfterimageIntensity, _AfterimageOffset;

            float hash11(float n) { return frac(sin(n) * 43758.5453123); }
            float hash12(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float3 HashColorDark(float seed)
            {
                float selector = hash11(seed + 55.0);
                float3 c;
                if (selector < 0.18) c = float3(1.0, 0.12, 0.16);
                else if (selector < 0.36) c = float3(0.16, 1.0, 0.22);
                else if (selector < 0.54) c = float3(0.18, 0.42, 1.0);
                else if (selector < 0.72) c = float3(1.0, 0.88, 0.10);
                else if (selector < 0.88) c = float3(0.10, 1.0, 1.0);
                else c = 0.70 + 0.30 * float3(hash11(seed + 1.37), hash11(seed + 7.91), hash11(seed + 19.43));
                return c * 0.60;
            }

            float EaseInOut(float x) { x = saturate(x); return x * x * (3.0 - 2.0 * x); }
            float EaseInStrong(float x) { x = saturate(x); return x * x * x; }
            float LineMask(float value, float center, float width, float softness) { return 1.0 - smoothstep(width, width + softness, abs(value - center)); }
            float SegmentMask(float x, float center, float halfLen, float fade) { return 1.0 - smoothstep(halfLen, halfLen + fade, abs(x - center)); }

            float GroupedHorizontalShake(float2 uv, float time, float jitter)
            {
                float groupCount = 22.0;
                float groupId = floor(uv.y * groupCount);
                float seed = groupId * 37.17 + _HoldSeed;

                float speedA = lerp(14.0, 72.0, hash11(seed + 1.0));
                float speedB = lerp(5.0, 28.0, hash11(seed + 2.0));
                float range = lerp(0.0015, 0.022, hash11(seed + 3.0));
                float direction = hash11(seed + 4.0) < 0.5 ? -1.0 : 1.0;

                float shake = sin(time * speedA + seed) * range;
                shake += sin(time * speedB + seed * 1.7) * range * 0.42;

                float pulseFrame = floor(time * lerp(7.0, 18.0, hash11(seed + 5.0)));
                float pulseActive = step(0.86, hash11(seed + pulseFrame * 13.13));
                float pulseOffset = (hash11(seed + pulseFrame * 7.71) * 2.0 - 1.0) * range * 1.85;

                return (shake * direction + pulseOffset * pulseActive) * jitter;
            }

            float FineLineShake(float2 uv, float time, float jitter)
            {
                float lineId = floor(uv.y * 96.0);
                float seed = lineId * 19.91 + _HoldSeed;
                float frame = floor(time * 18.0);
                float active = step(0.70, hash11(seed + frame * 11.37));
                float offset = (hash11(seed + frame * 21.0) * 2.0 - 1.0) * 0.006;
                return offset * active * jitter;
            }

            half4 SampleScene(float2 uv)
            {
                uv = saturate(uv);
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, UnityStereoTransformScreenSpaceTex(uv));
            }

            half4 SampleRGBSplit(float2 uv, float split, float offsetX)
            {
                float2 baseUv = uv + float2(offsetX, 0.0);
                half r = SampleScene(baseUv + float2(split, 0.0)).r;
                half g = SampleScene(baseUv).g;
                half b = SampleScene(baseUv - float2(split, 0.0)).b;
                return half4(r, g, b, 1.0);
            }

            float HorizontalTearOffset(float2 uv, float time, float intensity)
            {
                float rowCoarse = floor(uv.y * 22.0 + floor(time * 4.0));
                float rowMid = floor(uv.y * 78.0 + floor(time * 10.0));
                float rowThin = floor(uv.y * 185.0 + floor(time * 28.0));
                float a = hash11(rowCoarse * 17.17 + floor(time * 11.0));
                float b = hash11(rowMid * 41.41 + floor(time * 29.0));
                float c = hash11(rowThin * 79.13 + floor(time * 47.0));
                float coarseBand = step(0.72, a);
                float midBand = step(0.58, b) * 0.44;
                float thinBand = step(0.82, c) * 0.16;
                float wave = sin(uv.y * 28.0 + time * 8.0) * 0.15;
                return (((a * 2.0 - 1.0) * coarseBand) + ((b * 2.0 - 1.0) * midBand) + ((c * 2.0 - 1.0) * thinBand) + wave) * intensity;
            }

            float3 SceneGlitch(float2 uv, float amount)
            {
                amount = saturate(amount) * _SceneGlitchIntensity;
                float t = _ManualTime;
                float tear = HorizontalTearOffset(uv, t, _HorizontalTear * amount);
                float wave = (sin(uv.y * 155.0 + t * 31.0) + sin(uv.y * 26.0 - t * 9.0)) * _WaveDistortion * amount;
                float split = _RGBSplit * amount * (0.72 + 0.28 * sin(t * 44.0));
                half4 col = SampleRGBSplit(uv, split, tear + wave);
                float row = floor(uv.y * 92.0 + floor(t * 16.0));
                float rnd = hash11(row * 13.41 + floor(t * 23.0));
                float band = step(0.66, rnd);
                float shifted = (rnd * 2.0 - 1.0) * _HorizontalTear * 1.55 * amount;
                half4 streak = SampleRGBSplit(uv, split * 1.25, shifted);
                col.rgb = lerp(col.rgb, streak.rgb, band * 0.44 * amount);
                float grain = hash12(uv * float2(1733.0, 811.0) + floor(t * 62.0));
                col.rgb += (grain - 0.5) * _FineNoise * amount;
                return col.rgb;
            }

            float StarShape(float2 uv, float scale, out float glow)
            {
                float2 p = uv * 2.0 - 1.0;
                p.x *= _Aspect;
                p /= max(scale, 0.0001);
                float x = abs(p.x);
                float y = abs(p.y);
                float edge = max(0.0005, _StarEdge);
                float sharp = max(1.0, _StarSharpness);

                float hReach = max(0.001, _StarHorizontalReach);
                float hNorm = saturate(x / hReach);
                float hTaper = pow(saturate(1.0 - hNorm), sharp * 0.55);
                float hWidth = _StarTipWidth + _StarHorizontalThickness * hTaper;
                float horizontal = 1.0 - smoothstep(hWidth, hWidth + edge, y);
                horizontal *= 1.0 - smoothstep(hReach, hReach + edge * 12.0, x);

                float vReach = max(0.001, _StarVerticalReach);
                float vNorm = saturate(y / vReach);
                float vTaper = pow(saturate(1.0 - vNorm), sharp * 0.80);
                float vWidth = _StarTipWidth * 0.70 + _StarVerticalThickness * vTaper;
                float vertical = 1.0 - smoothstep(vWidth, vWidth + edge, x);
                vertical *= 1.0 - smoothstep(vReach, vReach + edge * 8.0, y);

                float core = exp(-pow(x / 0.55, 2.2) - pow(y / 0.18, 1.6));
                float star = saturate(max(max(horizontal, vertical), core));

                float hGlow = exp(-pow(x / 1.35, 1.4) - pow(y / 0.22, 2.0));
                float cGlow = exp(-pow(x / 0.78, 1.55) - pow(y / 0.30, 1.75));
                glow = saturate(max(hGlow * 0.55, cGlow * 0.40) - star * 0.95);
                return star;
            }

            float3 HeldReferenceGlitch(float2 uv, float appear, float jitter)
            {
                float2 p = uv;
                float t = _ManualTime;
                p.x += GroupedHorizontalShake(p, t, jitter);
                p.x += FineLineShake(p, t, jitter);

                float3 col = 0.0;
                float seed = _HoldSeed;

                // Compile-safe reference-style glitch:
                // no nested loops, no forced unroll. Thin horizontal strips + broken RGB fragments.
                [loop]
                for (int i = 0; i < 56; i++)
                {
                    float id = (float)i;
                    float s = seed + id * 29.731;
                    float cy = hash11(s + 1.0);
                    float height = lerp(0.00045, 0.0024, hash11(s + 2.0));
                    float band = LineMask(p.y, cy, height, height * 1.8 + 0.00035);

                    float cx = hash11(s + 3.0);
                    float halfLen = lerp(0.035, 0.22, hash11(s + 4.0));
                    float fade = lerp(0.006, 0.032, hash11(s + 5.0));
                    float3 color = HashColorDark(s);

                    float rShift = (hash11(s + 6.0) - 0.5) * 0.038;
                    float gShift = (hash11(s + 7.0) - 0.5) * 0.012;
                    float bShift = (hash11(s + 8.0) - 0.5) * 0.038;

                    float r = SegmentMask(p.x, cx + rShift, halfLen, fade);
                    float g = SegmentMask(p.x, cx + gShift, halfLen * lerp(0.84, 1.18, hash11(s + 9.0)), fade);
                    float b = SegmentMask(p.x, cx + bShift, halfLen * lerp(0.84, 1.18, hash11(s + 10.0)), fade);

                    float strength = lerp(0.28, 1.0, hash11(s + 11.0));
                    col += float3(color.r * r, color.g * g, color.b * b) * band * strength;
                }

                // Short block fragments layered over strips.
                [loop]
                for (int j = 0; j < 44; j++)
                {
                    float id = (float)j;
                    float s = seed + 1800.0 + id * 43.17;
                    float cy = hash11(s + 1.0);
                    float height = lerp(0.0011, 0.0052, hash11(s + 2.0));
                    float band = LineMask(p.y, cy, height, height * 1.45 + 0.0006);

                    float cx = hash11(s + 3.0);
                    float halfLen = lerp(0.010, 0.070, hash11(s + 4.0));
                    float fade = lerp(0.004, 0.017, hash11(s + 5.0));
                    float3 color = HashColorDark(s + 17.0) * lerp(0.70, 1.10, hash11(s + 6.0));

                    float r = SegmentMask(p.x, cx + (hash11(s + 7.0) - 0.5) * 0.024, halfLen, fade);
                    float g = SegmentMask(p.x, cx + (hash11(s + 8.0) - 0.5) * 0.009, halfLen, fade);
                    float b = SegmentMask(p.x, cx + (hash11(s + 9.0) - 0.5) * 0.024, halfLen, fade);
                    col += float3(color.r * r, color.g * g, color.b * b) * band * 0.85;
                }

                // A few longer accent strips.
                [loop]
                for (int a = 0; a < 8; a++)
                {
                    float id = (float)a;
                    float s = seed + 3600.0 + id * 67.29;
                    float cy = hash11(s + 1.0);
                    float height = lerp(0.00055, 0.0014, hash11(s + 2.0));
                    float band = LineMask(p.y, cy, height, height * 1.6 + 0.00035);
                    float cx = hash11(s + 3.0);
                    float halfLen = lerp(0.15, 0.40, hash11(s + 4.0));
                    float seg = SegmentMask(p.x, cx, halfLen, 0.018);
                    col += HashColorDark(s + 5.0) * band * seg * 0.85;
                }

                float grain = hash12(p * float2(1330.0, 770.0) + floor(t * 31.0));
                col += (grain - 0.5) * 0.007;
                col *= appear * _HoldBurstIntensity;
                return saturate(col * 0.95);
            }

            float3 PowerOnEcho(float2 uv, float progress)
            {
                float p = saturate(progress);
                float echoFade = smoothstep(0.10, 0.30, p) * (1.0 - smoothstep(0.94, 1.0, p));
                float echo = _AfterimageIntensity * echoFade;
                float offset = _AfterimageOffset * lerp(2.35, 0.08, smoothstep(0.0, 1.0, p));
                float3 mainCol = SampleScene(uv).rgb;
                float3 px = SampleRGBSplit(uv + float2(offset, 0.0), _RGBSplit * 0.55 * echo, 0.0).rgb;
                float3 nx = SampleRGBSplit(uv - float2(offset, 0.0), _RGBSplit * 0.55 * echo, 0.0).rgb;
                float3 py = SampleRGBSplit(uv + float2(0.0, offset * 0.75), _RGBSplit * 0.28 * echo, 0.0).rgb;
                float3 ny = SampleRGBSplit(uv - float2(0.0, offset * 0.75), _RGBSplit * 0.28 * echo, 0.0).rgb;
                float3 echoCol = px * float3(1.0, 0.20, 0.12) + nx * float3(0.16, 0.34, 1.0) + py * float3(0.28, 1.0, 0.28) + ny * float3(1.0, 0.88, 0.18);
                echoCol *= 0.26;
                return mainCol + echoCol * echo;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float mode = floor(_Mode + 0.5);
                float progress = saturate(_Progress);
                float eased = EaseInOut(progress);
                float easedIn = EaseInStrong(progress);

                if (mode < 0.5) return SampleScene(uv);

                if (mode < 1.5)
                {
                    float amount = lerp(0.05, 1.0, easedIn);
                    return half4(SceneGlitch(uv, amount), 1.0);
                }

                if (mode < 2.5)
                {
                    float glow;
                    float scale = lerp(1.18, 0.05, eased);
                    float star = StarShape(uv, scale, glow);
                    float3 col = (star * _StarIntensity + glow * _StarGlowIntensity) * float3(1.0, 1.0, 1.0);
                    return half4(col, 1.0);
                }

                if (mode < 3.5)
                {
                    float appear = eased;
                    float jitter = 1.0 - eased;
                    return half4(HeldReferenceGlitch(uv, appear, jitter), 1.0);
                }

                if (mode < 4.5)
                {
                    float flicker = 0.72 + 0.28 * step(0.42, hash11(floor(_ManualTime * 24.0) + _HoldSeed));
                    float dissolve = (1.0 - eased) * flicker;
                    return half4(HeldReferenceGlitch(uv, dissolve, 0.22 * (1.0 - eased)), 1.0);
                }

                if (mode < 5.5)
                {
                    return half4(0.0, 0.0, 0.0, 1.0);
                }

                float p = progress;
                float glowRise = smoothstep(0.03, 0.48, p);
                float glowFall = 1.0 - smoothstep(0.42, 0.88, p);
                float flash = glowRise * glowFall * _FlashIntensity;
                float sceneReveal = smoothstep(0.18, 0.80, p);
                float3 sceneEcho = PowerOnEcho(uv, p);
                float2 centerUv = uv * 2.0 - 1.0;
                centerUv.x *= _Aspect;
                float radialRadius = lerp(1.10, 0.62, smoothstep(0.0, 0.55, p));
                float radial = exp(-pow(length(centerUv) / radialRadius, 2.2));
                float3 flashCol = float3(1.0, 1.0, 1.0) * flash;
                flashCol += float3(1.0, 1.0, 1.0) * radial * flash * 1.35;
                float3 col = lerp(float3(0.0, 0.0, 0.0), sceneEcho, sceneReveal);
                col += flashCol;
                col = lerp(col, SampleScene(uv).rgb, smoothstep(0.985, 1.0, p));
                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
