Shader "Custom/WaterShader"
{
    Properties
    {
        _Color ("Deep Water Color", Color) = (0.1, 0.3, 0.5, 0.9)
        _ShallowColor ("Shallow Water Color", Color) = (0.3, 0.65, 0.9, 0.5)
        _MainTex ("Normal Map", 2D) = "bump" {}
        _WaveSpeed ("Wave Speed", Range(0, 2)) = 0.5
        _WaveHeight ("Wave Height", Range(0, 1)) = 0.1
        _Glossiness ("Smoothness", Range(0,1)) = 0.8
        _FresnelPower ("Fresnel Power", Range(1, 10)) = 5
        _ShoreDistance ("Shore Blend Distance", Range(0, 10)) = 3
        _ShoreBlend ("Shore Blend", Range(0, 5)) = 1
        _DepthMaxDistance ("Maximum Depth Distance", Float) = 1

        [Enum(Equal,3,NotEqual,6)] _StencilTest ("Stencil Test", int) = 6

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        Stencil
        {
            Ref 1
            Comp [_StencilTest]
        }

        GrabPass { "_GrabTexture" }

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        sampler2D _GrabTexture;
        sampler2D _CameraDepthTexture;
        fixed4 _Color;
        fixed4 _ShallowColor;
        float _WaveSpeed;
        float _WaveHeight;
        float _Glossiness;
        float _FresnelPower;
        float _ShoreDistance;
        float _ShoreBlend;
        float _DepthMaxDistance;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Screen UV for depth sampling
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            
            // Get scene depth
            float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV));
            float surfaceDepth = IN.screenPos.w;
            float depthDifference = saturate((sceneDepth - surfaceDepth) / _DepthMaxDistance);

            // Create scrolling UVs for wave animation
            float2 uv1 = IN.uv_MainTex + _Time.y * float2(_WaveSpeed, 0);
            float2 uv2 = IN.uv_MainTex + _Time.y * float2(-_WaveSpeed * 0.7, _WaveSpeed * 0.5);
            
            // Sample normal map twice for more complex wave effect
            float3 normal1 = UnpackNormal(tex2D(_MainTex, uv1));
            float3 normal2 = UnpackNormal(tex2D(_MainTex, uv2));
            float3 finalNormal = normalize(normal1 + normal2);
            
            // Apply wave height
            finalNormal *= _WaveHeight;
            
            // Calculate fresnel effect
            float fresnel = pow(1.0 - saturate(dot(finalNormal, IN.viewDir)), _FresnelPower);

            // Blend between shallow and deep water colors based on depth
            float shoreBlend = 1 - saturate(depthDifference * _ShoreBlend);
            float3 waterColor = lerp(_Color.rgb, _ShallowColor.rgb, shoreBlend);
            
            // Apply colors and effects
            o.Albedo = waterColor;
            o.Normal = finalNormal;
            o.Smoothness = _Glossiness;
            
            // Fade out alpha near shores
            float shoreAlpha = saturate(depthDifference * _ShoreDistance);
            o.Alpha = lerp(_ShallowColor.a, _Color.a, shoreAlpha) + fresnel * 0.3;
        }
        ENDCG
    }
    FallBack "Diffuse"
}