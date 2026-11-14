
Shader "Custom/Lesson10_Fresnel"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.08, 0.12, 0.2, 1)
        _FresnelColor("Fresnel Color", Color) = (0.1, 0.8, 1, 1)

        _Power("Rim Power", Range(0.5, 8)) = 3
        _Intensity("Rim Intensity", Range(0, 3)) = 1
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.1
        _RimSoftness ("Rim Softness", Range(0.01, 1)) = 0.3
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
            Name "FresnelUnlit"

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _FresnelColor;

                float _Power;
                float _Intensity;
                float _RimThreshold;
                float _RimSoftness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;

                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // 1. 获取顶点在不同坐标空间中的位置
                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(IN.positionOS.xyz);

                // 2. 将法线转换到世界空间
                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.normalWS = normalInputs.normalWS;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 3. 获取单位法线
                float3 N = SafeNormalize(IN.normalWS);

                // 4. 获取从当前片元指向摄像机的单位向量
                float3 V =
                    GetWorldSpaceNormalizeViewDir(IN.positionWS);

                // 5. 计算法线与观察方向的点积
                float NdotV = saturate(dot(N, V));

                // 6. 计算边缘遮罩
                float edge = 1.0 - NdotV;
                
                float rimMask = smoothstep(
                    _RimThreshold,
                    _RimThreshold + _RimSoftness,
                    edge
                );
                
                float3 F0 = float3(0.04,0.04,0.04);
                
                float fresnel = F0 + (1-F0) * pow(rimMask,5);
                
                // 7. 使用幂函数控制边缘范围
                //float fresnel = pow(edge, _Power);
                
                // 8. 将边缘颜色叠加到基础颜色
                float3 finalColor =
                    _BaseColor.rgb +
                    _FresnelColor.rgb * fresnel * _Intensity;

                return half4(finalColor, _BaseColor.a);
            }

            ENDHLSL
        }
    }
}
