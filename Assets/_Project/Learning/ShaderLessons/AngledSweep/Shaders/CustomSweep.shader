Shader "Custom/CustomSweep"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Angle("Angle",Float) = 45
        _Speed("Speed",Float) = 0.2
        _Width("Width",Float) = 0.1
        _SweepColor("Sweep Color",Color) = (1,1,1,1)
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
                half4 _SweepColor;
                float4 _BaseMap_ST;
                float _Angle;
                float _Speed;
                float _Width;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                float rad = radians(_Angle);
                
                float2 tangent = float2(
                    cos(rad),
                    sin(rad)
                );
                
                float2 normal = float2(
                    -sin(rad),
                    cos(rad)
                );

                float x = dot(uv, tangent);
                float y = dot(uv, normal);
                
                // 1. 计算按钮沿扫描方向的投影范围
                float minY =
                    min(0.0, normal.x) +
                    min(0.0, normal.y);
                
                float maxY =
                    max(0.0, normal.x) +
                    max(0.0, normal.y);
                
                // 2. 扫描线从按钮外部开始和结束
                float start = minY - _Width;
                float end = maxY + _Width;
                
                // 3. 生成 0~1 的周期动画进度
                float progress = frac(_Speed * _Time.y);
                
                // 4. 根据动画进度计算唯一的扫描线中心
                float center = lerp(start, end, progress);
                
                // 5. 当前像素到扫描线中心的距离
                float distance = abs(y - center);
                
                float mask = 1 - step(_Width,distance);
                
                float3 color = lerp(_BaseColor.rgb,_SweepColor.rgb,mask);
                
                return float4(color,1);
            }
            ENDHLSL
        }
    }
}
