
Shader "Study/09 Phong And BlinnPhong"
{
    Properties
    {
        _BaseColor("基础颜色", Color) = (0.5, 0.5, 0.5, 1)

        _SpecColor("高光颜色", Color) = (1, 1, 1, 1)

        _Shininess("高光锐度", Range(1, 128)) = 32

        _SpecularStrength("高光强度", Range(0, 2)) = 1

        _AmbientStrength("环境光强度", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)

                float4 _BaseColor;
                float4 _SpecColor;

                float _Shininess;
                float _SpecularStrength;
                float _AmbientStrength;

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

            // 顶点着色器
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // 物体空间 -> 世界空间
                OUT.positionWS =
                    TransformObjectToWorld(IN.positionOS.xyz);

                // 法线转换到世界空间
                OUT.normalWS =
                    TransformObjectToWorldNormal(IN.normalOS);

                // 世界空间 -> 裁剪空间
                OUT.positionCS =
                    TransformWorldToHClip(OUT.positionWS);

                return OUT;
            }

            // 片元着色器
            half4 frag(Varyings IN) : SV_Target
            {
                // 1. 获取法线
                float3 N = SafeNormalize(IN.normalWS);

                // 2. 获取主光源
                Light mainLight = GetMainLight();

                // 指向光源的方向
                float3 L = SafeNormalize(mainLight.direction);

                // 3. 获取观察方向
                float3 V = SafeNormalize(
                    GetWorldSpaceViewDir(IN.positionWS)
                );

                // 4. Lambert 漫反射
                float NdotL = saturate(dot(N, L));

                float3 diffuse =
                    _BaseColor.rgb *
                    mainLight.color *
                    NdotL;

                // 5. 计算镜面高光
                float specularTerm = 0.0;
                
                // Blinn-Phong

                float3 H = SafeNormalize(L + V);

                specularTerm = pow(
                    saturate(dot(N, H)),
                    _Shininess
                );

                // 6. 高光颜色
                float3 specular =
                    _SpecColor.rgb *
                    mainLight.color *
                    specularTerm *
                    _SpecularStrength;

                // 7. 环境光
                float3 ambient =
                    SampleSH(N) *
                    _BaseColor.rgb *
                    _AmbientStrength;

                // 8. 最终颜色
                float3 finalColor =
                    ambient + diffuse + specular;

                return half4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}
