Shader "Study/01 UV Visualization And Gradients"
{
    Properties
    {
        [Enum(Solid,0,UV,1,Horizontal,2,Vertical,3,Diagonal,4,FourCorners,5,SmoothHorizontal,6)]
        _Mode ("Display Mode", Float) = 1

        _ColorA ("A - Bottom Left", Color) = (1,0,0,1)
        _ColorB ("B - Bottom Right", Color) = (0,0,1,1)
        _ColorC ("C - Top Left", Color) = (0,1,0,1)
        _ColorD ("D - Top Right", Color) = (1,1,0,1)

        _UVScale ("UV Scale (XY)", Vector) = (1,1,0,0)
        _UVOffset ("UV Offset (XY)", Vector) = (0,0,0,0)

        _EdgeStart ("Smooth Start", Range(0,1)) = 0.2
        _EdgeEnd ("Smooth End", Range(0,1)) = 0.8
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
            Name "GradientUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float4 _ColorC;
                float4 _ColorD;

                float4 _UVScale;
                float4 _UVOffset;

                float _Mode;
                float _EdgeStart;
                float _EdgeEnd;
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
                float2 uv =
                    IN.uv * _UVScale.xy + _UVOffset.xy;

                float2 uv01 = saturate(uv);

                // 模式 0：纯色
                if (_Mode < 0.5)
                    return _ColorA;

                // 模式 1：UV 可视化
                if (_Mode < 1.5)
                    return float4(uv01.x, uv01.y, 0, 1);

                // 模式 2：水平渐变
                if (_Mode < 2.5)
                    return lerp(_ColorA, _ColorB, uv01.x);

                // 模式 3：垂直渐变
                if (_Mode < 3.5)
                    return lerp(_ColorA, _ColorB, uv01.y);

                // 模式 4：对角线渐变
                if (_Mode < 4.5)
                {
                    float t = saturate(
                        (uv.x + uv.y) * 0.5
                    );

                    return lerp(_ColorA, _ColorB, t);
                }

                // 模式 5：四角渐变
                if (_Mode < 5.5)
                {
                    float4 bottom =
                        lerp(_ColorA, _ColorB, uv01.x);

                    float4 top =
                        lerp(_ColorC, _ColorD, uv01.x);

                    return lerp(bottom, top, uv01.y);
                }

                // 模式 6：平滑水平渐变
                float tSmooth = smoothstep(
                    _EdgeStart,
                    _EdgeEnd,
                    uv.x
                );

                return lerp(_ColorA, _ColorB, tSmooth);
            }

            ENDHLSL
        }
    }
}