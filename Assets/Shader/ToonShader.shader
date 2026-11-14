
Shader "Custom/Lesson13_ToonLighting"
{
    Properties
    {
        _BaseMap ("基础纹理 Base Map", 2D) = "white" {}
        _BaseColor ("基础颜色 Base Color", Color) = (1,1,1,1)

        _ShadowColor ("暗部色 Shadow", Color) = (0.22,0.30,0.45,1)
        _MidColor ("中间色 Mid", Color) = (0.72,0.80,0.88,1)
        _LightColor ("亮部色 Light", Color) = (1,1,1,1)
        _Light2Color ("亮部色 Light", Color) = (1,1,1,1)

        _ShadowThreshold ("暗部阈值", Range(0,1)) = 0.3
        _LightThreshold ("亮部阈值", Range(0,1)) = 0.7
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
            Name "ToonForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;

                half4 _BaseColor;
                half4 _ShadowColor;
                half4 _MidColor;
                half4 _LightColor;
                half4 _Light2Color;
            
                float _ShadowThreshold;
                float _LightThreshold;
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
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionCS =
                    TransformObjectToHClip(IN.positionOS.xyz);

                OUT.normalWS =
                    TransformObjectToWorldNormal(IN.normalOS);

                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. 得到世界空间法线
                float3 normalWS = normalize(IN.normalWS);

                // 2. 获取主光源
                Light mainLight = GetMainLight();

                float3 lightDirWS =
                    normalize(mainLight.direction);

                // 3. 连续的 Lambert 光照值
                float NdotL =
                    saturate(dot(normalWS, lightDirWS));

                float test = NdotL * 4;
                
                float edge = floor(test);
                
                // 4. 判断进入哪个色阶
                float enterMid =
                    step(1, NdotL);

                float enterLight =
                    step(2, NdotL);
                
                float enter3Light =
                    step(3, NdotL);
                
                // 5. 选择卡通光照颜色
                half3 toonColor =
                    lerp(_ShadowColor.rgb,
                         _MidColor.rgb,
                         enterMid);

                toonColor =
                    lerp(toonColor,
                         _LightColor.rgb,
                         enterLight);

                // 6. 采样基础纹理
                half3 baseColor =
                    SAMPLE_TEXTURE2D(
                        _BaseMap,
                        sampler_BaseMap,
                        IN.uv
                    ).rgb * _BaseColor.rgb;

                // 7. 最终颜色
                half3 finalColor =
                    baseColor *
                    toonColor *
                    mainLight.color;

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
}
