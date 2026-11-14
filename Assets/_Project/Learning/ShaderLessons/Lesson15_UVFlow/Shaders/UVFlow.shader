
Shader "Custom/15_UVFlow"
{
    Properties
    {
        _FlowMapA("第一层纹理", 2D) = "white" {}
        _FlowMapB("第二层纹理", 2D) = "white" {}

        _Tint("整体颜色", Color) = (1,1,1,1)

        _DirectionA("第一层方向 XY", Vector) = (1,0,0,0)
        _SpeedA("第一层速度", Range(0,2)) = 0.2

        _DirectionB("第二层方向 XY", Vector) = (0,1,0,0)
        _SpeedB("第二层速度", Range(0,2)) = 0.1

        _Blend("双层混合比例", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }

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

            TEXTURE2D(_FlowMapA);
            SAMPLER(sampler_FlowMapA);

            TEXTURE2D(_FlowMapB);
            SAMPLER(sampler_FlowMapB);

            CBUFFER_START(UnityPerMaterial)

                float4 _FlowMapA_ST;
                float4 _FlowMapB_ST;

                float4 _Tint;

                float4 _DirectionA;
                float4 _DirectionB;

                float _SpeedA;
                float _SpeedB;

                float _Blend;

            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS =
                    TransformObjectToHClip(IN.positionOS.xyz);

                OUT.uv = IN.uv;

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // 1. 获取经过 Tiling/Offset 的 UV
                float2 uvA =
                    TRANSFORM_TEX(IN.uv, _FlowMapA);

                float2 uvB =
                    TRANSFORM_TEX(IN.uv, _FlowMapB);

                // 2. 归一化运动方向
                float2 dirA = _DirectionA.xy /
                    max(length(_DirectionA.xy), 0.00001);

                float2 dirB = _DirectionB.xy /
                    max(length(_DirectionB.xy), 0.00001);

                // 3. 计算两层纹理的累计位移
                float2 offsetA =
                    dirA * _SpeedA * _Time.y;

                float2 offsetB =
                    dirB * _SpeedB * _Time.y;

                // 4. 反向查找源纹理坐标
                float2 flowUVA = uvA - offsetA;
                float2 flowUVB = uvB - offsetB;

                // 5. 分别采样纹理
                float4 colorA = SAMPLE_TEXTURE2D(
                    _FlowMapA,
                    sampler_FlowMapA,
                    flowUVA
                );

                float4 colorB = SAMPLE_TEXTURE2D(
                    _FlowMapB,
                    sampler_FlowMapB,
                    flowUVB
                );

                // 6. 混合两层
                float4 result =
                    lerp(colorA, colorB, _Blend);

                return result * _Tint;
            }

            ENDHLSL
        }
    }
}
