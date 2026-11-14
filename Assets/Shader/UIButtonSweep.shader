
Shader "Custom/UI/ButtonSweep"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // 扫光参数
        _SweepColor ("Sweep Color", Color) = (1,1,1,1)
        _Angle ("Angle", Range(-180,180)) = 45
        _Width ("Width", Range(0.01,1)) = 0.25
        _Intensity ("Intensity", Range(0,3)) = 1.5
        _Speed ("Speed (Cycles/s)", Range(0,3)) = 0.4
        [Toggle] _Reverse ("Reverse Direction", Float) = 0

        // UI Rect 信息：(xMin, yMin, width, height)
        _RectInfo ("Rect Info", Vector) = (-50,-50,100,100)

        // UGUI 遮罩支持
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UI Sweep"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float4 _SweepColor;
            float4 _TextureSampleAdd;

            float4 _RectInfo;
            float4 _ClipRect;

            float _Angle;
            float _Width;
            float _Intensity;
            float _Speed;
            float _Reverse;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 localPos : TEXCOORD1;
                float4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                // 顶点位置转换到裁剪空间
                output.positionCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                // 保留采样 UV
                output.uv = input.uv;

                // 保留 UI 的局部坐标
                output.localPos = input.positionOS.xy;

                // UI Image 的颜色
                output.color = input.color * _Color;

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // ====================================
                // 1. 获取按钮原始颜色
                // ====================================

                float4 col =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv
                    );

                col = (col + _TextureSampleAdd) * input.color;

                // ====================================
                // 2. 获取按钮局部 UV（0~1）
                // ====================================

                float2 size = max(
                    _RectInfo.zw,
                    float2(0.0001, 0.0001)
                );

                float2 uv =
                    (input.localPos - _RectInfo.xy) / size;

                // ====================================
                // 3. 中心化 + 宽高比修正
                // ====================================

                float aspect = size.x / size.y;

                float2 p =
                    (uv - 0.5) * float2(aspect, 1.0);

                // ====================================
                // 4. 根据角度计算亮带方向
                // ====================================

                float rad = radians(_Angle);

                float2 tangent = float2(
                    cos(rad),
                    sin(rad)
                );

                // 亮带的法向量
                float2 normal = float2(
                    -tangent.y,
                    tangent.x
                );

                // ====================================
                // 5. 将当前像素投影到法向方向
                // ====================================

                float projected = dot(p, normal);

                // ====================================
                // 6. 计算扫描范围
                // ====================================

                float extent = 0.5 * (
                    abs(normal.x) * aspect +
                    abs(normal.y)
                );

                // ====================================
                // 7. 时间控制亮带位置
                // ====================================

                float progress = frac(_Time.y * _Speed);

                progress = lerp(
                    progress,
                    1.0 - progress,
                    _Reverse
                );

                float width = max(_Width, 0.0001);

                float center = lerp(
                    -extent - width,
                     extent + width,
                     progress
                );

                // ====================================
                // 8. 计算当前像素到亮带的距离
                // ====================================

                float d = abs(projected - center);

                // ====================================
                // 9. 将距离映射为亮度
                // ====================================

                float light =
                    1.0 - smoothstep(0.0, width, d);

                // ====================================
                // 10. 将扫光叠加到原始按钮颜色
                // ====================================

                col.rgb = saturate(
                    col.rgb +
                    _SweepColor.rgb *
                    _SweepColor.a *
                    light *
                    _Intensity
                );

                // ====================================
                // 11. 支持 UGUI RectMask2D
                // ====================================

                #ifdef UNITY_UI_CLIP_RECT

                float2 inside =
                    step(_ClipRect.xy, input.localPos) *
                    step(input.localPos, _ClipRect.zw);

                col.a *= inside.x * inside.y;

                #endif

                // ====================================
                // 12. 支持 UI Alpha Clip
                // ====================================

                #ifdef UNITY_UI_ALPHACLIP
                    clip(col.a - 0.001);
                #endif

                return col;
            }

            ENDHLSL
        }
    }
}
