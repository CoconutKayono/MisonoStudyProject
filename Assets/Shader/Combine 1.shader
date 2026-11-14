Shader "Custom/Practice07_Dissolve"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)

        _NoiseScale("Noise Scale", Range(1,40)) = 12
        _Progress("Dissolve Progress", Range(0,1)) = 0
        _EdgeWidth("Edge Width", Range(0.005,0.25)) = 0.08

        [HDR]_EdgeInnerColor("Edge Inner Color", Color)
            = (3,1.5,0.1,1)

        [HDR]_EdgeOuterColor("Edge Outer Color", Color)
            = (1,0.12,0.02,1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "TransparentCutout"
            "Queue" = "AlphaTest"
        }

        Pass
        {
            Name "Dissolve"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Back
            ZWrite On
            Blend Off

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
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _EdgeInnerColor;
                float4 _EdgeOuterColor;
                float _NoiseScale;
                float _Progress;
                float _EdgeWidth;
            CBUFFER_END

            // 生成一个 0~1 的伪随机数
            float Hash21(float2 p)
            {
                return frac(
                    sin(dot(p, float2(127.1, 311.7)))
                    * 43758.5453
                );
            }

            // 二维 Value Noise
            float ValueNoise(float2 p)
            {
                float2 cell = floor(p);
                float2 local = frac(p);

                // 三次 Hermite 平滑插值
                float2 w = local * local * (3.0 - 2.0 * local);

                float a = Hash21(cell);
                float b = Hash21(cell + float2(1, 0));
                float c = Hash21(cell + float2(0, 1));
                float d = Hash21(cell + float2(1, 1));

                float bottom = lerp(a, b, w.x);
                float top = lerp(c, d, w.x);

                return lerp(bottom, top, w.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionCS =
                    TransformObjectToHClip(IN.positionOS.xyz);

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 第一步：采样原始纹理
                float3 baseColor =
                    SAMPLE_TEXTURE2D(
                        _BaseMap,
                        sampler_BaseMap,
                        IN.uv
                    ).rgb * _BaseColor.rgb;

                // 第二步：产生固定的空间噪声
                float noise =
                    ValueNoise(IN.uv * _NoiseScale);

                // 第三步：确定裁切阈值
                float width = max(_EdgeWidth, 0.0001);

                // 扩展端点，确保 0 全保留、1 全消失
                float threshold = lerp(
                    -width - 0.001,
                    1.001,
                    saturate(_Progress)
                );

                // 第四步：计算相对边界值
                float f = noise - threshold;

                // 丢弃噪声值低于阈值的片元
                clip(f);

                // 第五步：归一化边缘范围
                float edgeT = saturate(f / width);

                // 第六步：平滑的边缘遮罩
                float edgeMask =
                    1.0 - smoothstep(0.0, 1.0, edgeT);

                // 第七步：双颜色边缘渐变
                float3 edgeColor = lerp(
                    _EdgeInnerColor.rgb,
                    _EdgeOuterColor.rgb,
                    edgeT
                );

                // 第八步：将边缘着色与原始纹理合成
                float3 finalColor =
                    lerp(baseColor, edgeColor, edgeMask);

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
}