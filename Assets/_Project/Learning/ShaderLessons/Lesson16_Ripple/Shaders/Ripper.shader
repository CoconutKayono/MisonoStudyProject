
Shader "ShaderPractice/16_Ripple"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)

        _Center("Ripple Center UV", Vector) = (0.5,0.5,0,0)

        _Amplitude("Amplitude", Range(0,0.05)) = 0.015
        _Wavelength("Wavelength", Range(0.02,0.5)) = 0.12
        _Speed("Wave Speed", Range(-2,2)) = 0.3

        _FadeStart("Fade Start", Range(0,1)) = 0.15
        _FadeEnd("Fade End", Range(0,1)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float4 _Center;

                float _Amplitude;
                float _Wavelength;
                float _Speed;
                float _FadeStart;
                float _FadeEnd;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS =
                    TransformObjectToHClip(IN.positionOS.xyz);

                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. 原始 UV
                float2 uv = IN.uv;

                // 2. 圆心到当前片元的位置差
                float2 offset = uv - _Center.xy;

                // 3. 计算径向距离
                float distance = length(offset);

                // 4. 计算从圆心指向当前片元的方向
                float2 direction =
                    offset / max(distance, 0.0001);

                // 5. 获取当前时间
                float time = _Time.y;

                // 6. 计算波纹相位
                float phase =
                    6.2831853 *
                    (distance - _Speed * time)
                    / max(_Wavelength, 0.0001);

                // 7. 正弦波，范围 [-1,1]
                float wave = sin(phase);

                // 8. 圆心附近逐渐增强
                float centerFade =
                    smoothstep(0.0, 0.03, distance);

                // 9. 远离圆心逐渐减弱
                float distanceFade =
                    1.0 - smoothstep(
                        _FadeStart,
                        max(_FadeEnd, _FadeStart + 0.0001),
                        distance
                    );

                // 10. 最终扰动强度
                float strength =
                    wave *
                    _Amplitude *
                    centerFade *
                    distanceFade;

                // 11. 计算扭曲后的 UV
                float2 distortedUV =
                    uv + direction * strength;

                // 12. 应用纹理 Tiling / Offset
                float2 sampleUV =
                    TRANSFORM_TEX(distortedUV, _BaseMap);

                // 13. 使用新 UV 采样纹理
                half4 color =
                    SAMPLE_TEXTURE2D(
                        _BaseMap,
                        sampler_BaseMap,
                        sampleUV
                    );

                return color * _BaseColor;
            }

            ENDHLSL
        }
    }
}
