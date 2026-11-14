Shader "Custom/Combine"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _MossMap("Moss Map", 2D) = "white" {}
        _StoneMap("Stone Map", 2D) = "white" {}
        _MaskMap("Mask Map", 2D) = "white" {}
        _Feather("Feather",Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

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

            TEXTURE2D(_MossMap);
            SAMPLER(sampler_MossMap);
            TEXTURE2D(_StoneMap);
            SAMPLER(sampler_StoneMap);
            TEXTURE2D(_MaskMap);
            SAMPLER(sampler_MaskMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _MossMap_ST;
                float4 _StoneMap_ST;
                float4 _MaskMap_ST;
                float _Feather;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MossMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 mossColor = SAMPLE_TEXTURE2D(_MossMap, sampler_MossMap, IN.uv).rgb;
                half3 stoneColor = SAMPLE_TEXTURE2D(_StoneMap, sampler_StoneMap, IN.uv).rgb;
                half mask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, IN.uv).r;
                
                half _Progress = saturate(1 - abs(2 * frac(_Time.y * 0.2) - 1));
                
                float feather = _Feather;
                
                float threshold = lerp(
                    1.0 + feather,
                    -feather,
                    _Progress
                );
                
                float weight = smoothstep(
                    threshold - feather * 0.5,
                    threshold + feather * 0.5,
                    mask
                );
                
                half3 color = lerp(stoneColor,mossColor,weight);
                
                return half4(color,1);
            }
            ENDHLSL
        }
    }
}
