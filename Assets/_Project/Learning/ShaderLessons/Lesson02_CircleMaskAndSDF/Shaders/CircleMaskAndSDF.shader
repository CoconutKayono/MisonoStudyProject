Shader "Study/02 Circle Mask And SDF"
{
    Properties
    {
        [Enum(UV,0,Distance,1,SignedDistance,2,HardCircle,3,SoftCircle,4,HardRing,5,SoftRing,6)]
        _Mode ("Display Mode", Float) = 4

        _Color ("Circle Color", Color) = (0.1,0.7,1,1)

        _Center ("Circle Center (XY)", Vector) =
            (0.5,0.5,0,0)

        _Radius ("Circle Radius (UV)",
            Range(0,0.7)) = 0.3

        _SoftWidth ("Soft Edge Width (UV)",
            Range(0.001,0.2)) = 0.05

        _RingWidth ("Ring Width (UV)",
            Range(0.001,0.4)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "CircleMaskUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _Center;
                float _Mode;
                float _Radius;
                float _SoftWidth;
                float _RingWidth;
            CBUFFER_END

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS =
                    TransformObjectToHClip(IN.positionOS);

                OUT.uv = IN.uv;

                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // 1. 计算当前点到圆心的位移
                float2 p = uv - _Center.xy;

                // 2. 计算到圆心的距离
                float d = length(p);

                // 3. 计算到圆周的有符号距离
                float sd = d - _Radius;

                // 模式 0：UV 可视化
                if (_Mode < 0.5)
                {
                    return float4(
                        saturate(uv.x),
                        saturate(uv.y),
                        0, 1
                    );
                }

                // 模式 1：圆心距离可视化
                if (_Mode < 1.5)
                {
                    float v = saturate(d);
                    return float4(v, v, v, 1);
                }

                // 模式 2：有符号距离可视化
                if (_Mode < 2.5)
                {
                    // sd = 0 时显示 0.5 灰度
                    float v = saturate(0.5 + sd);

                    return float4(v, v, v, 1);
                }

                float mask;

                // 模式 3：硬边圆形
                if (_Mode < 3.5)
                {
                    mask = 1.0 - step(_Radius, d);
                }

                // 模式 4：软边圆形
                else if (_Mode < 4.5)
                {
                    float h = _SoftWidth * 0.5;

                    mask = 1.0 - smoothstep(
                        -h, h, sd
                    );
                }

                // 模式 5：硬边圆环
                else if (_Mode < 5.5)
                {
                    mask = 1.0 - step(
                        _RingWidth * 0.5,
                        abs(sd)
                    );
                }

                // 模式 6：软边圆环
                else
                {
                    float halfRing = _RingWidth * 0.5;
                    float halfSoft = _SoftWidth * 0.5;

                    mask = 1.0 - smoothstep(
                        halfRing - halfSoft,
                        halfRing + halfSoft,
                        abs(sd)
                    );
                }

                // 把遮罩应用于透明度
                return float4(
                    _Color.rgb,
                    _Color.a * mask
                );
            }

            ENDHLSL
        }
    }
}