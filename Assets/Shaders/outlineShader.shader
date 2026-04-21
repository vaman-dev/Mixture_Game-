Shader "Custom/URP/GhostOutline"
{
    Properties
    {
        _BaseColor ("Ghost Color", Color) = (0,1,1,0.3)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 0.03
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalRenderPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        // =========================
        // 🔲 PASS 1: OUTLINE
        // =========================
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }

            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            Varyings vert (Attributes v)
            {
                Varyings o;

                float3 normalWS = normalize(TransformObjectToWorldNormal(v.normalOS));
                float3 posWS = TransformObjectToWorld(v.positionOS.xyz);

                // Expand mesh
                posWS += normalWS * _OutlineThickness;

                o.positionHCS = TransformWorldToHClip(posWS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }

        // =========================
        // 👻 PASS 2: GHOST BODY
        // =========================
        Pass
        {
            Name "Ghost"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _BaseColor;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _BaseColor;
            }

            ENDHLSL
        }
    }
}