Shader "Custom/UV扫光"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. 获取时间
                float t = _Time.y;
            
                // 2. 设置速度
                float speed = 0.2;
            
                // 3. 计算移动后的周期坐标
                //float movedV = frac(IN.uv.y - speed * t);
                
                // 当前正在着色的片元位置
                float currentV = IN.uv.y;
                
                // 图案已经移动的距离
                float offset = speed * t;
                
                // 去原始图案中的哪个位置查询
                float oldV = currentV - offset;
                
                // 将查询坐标转换到重复的 [0,1) 区间
                float movedV = frac(oldV);
                
                // 4. 计算当前像素到扫描线中心的距离
                float d = abs(movedV - 0.5);
                
                // 5. 使用 smoothstep 制作软边遮罩
                float mask = 1.0 - smoothstep(0.02, 0.08, d);
                
                // 6. 定义背景色与扫光颜色
                half3 background = half3(0.08, 0.12, 0.25);
                half3 lightColor = half3(1.0, 0.9, 0.3);
                
                // 7. 根据遮罩混合颜色
                half3 finalColor = lerp(background, lightColor, mask);
                
                return half4(finalColor, 1);
                
                // float movedV = frac(IN.uv.y - speed * t);
                //
                // float Red = 1 - step(0.01,movedV);
                //
                // return float4(Red,0,0,1);
            }
            ENDHLSL
        }
    }
}
