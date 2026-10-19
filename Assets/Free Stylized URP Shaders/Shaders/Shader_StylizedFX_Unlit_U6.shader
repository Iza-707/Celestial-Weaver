Shader "StylizedFX/Unlit_Optimized"
{
    Properties
    {
        [Header(Main Settings)]
        [HDR] [MainColor] _BaseColor("Base Color (HDR)", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Main Texture (RGBA)", 2D) = "white" {}
        _MainTexUSpeed("Main Tex U Speed", Float) = 0
        _MainTexVSpeed("Main Tex V Speed", Float) = 0

        [Header(Noise Settings for Distortion and Dissolve)]
        _NoiseTex("Noise Texture (Grayscale)", 2D) = "white" {}
        _NoiseUSpeed("Noise U Speed", Float) = 0.2
        _NoiseVSpeed("Noise V Speed", Float) = 0.1
        
        [Header(VFX Features)]
        _DistortionStrength("Distortion Strength", Range(0, 1)) = 0.1
        _DissolveAmount("Dissolve Amount", Range(0, 1.01)) = 0
        _DissolveEdgeWidth("Dissolve Edge Width", Range(0.01, 0.5)) = 0.1
        [HDR] _DissolveEdgeColor("Dissolve Edge Color", Color) = (2,1,0,1)

        [Header(Fresnel Settings)]
        [Toggle(_FRESNEL_ON)] _UseFresnel("Enable Fresnel", Float) = 0
        _FresnelPower("Fresnel Power", Range(0.1, 10)) = 2
        [HDR] _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        
        [Header(Blending)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 5 
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dest Blend", Float) = 10 
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode - Double Sided", Float) = 0 
        [Toggle(_ZWRITE_ON)] _ZWrite("ZWrite", Float) = 0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector"="True"
        }

        Blend [_SrcBlend] [_DstBlend]
        Cull [_Cull]
        ZWrite [_ZWrite]

        Pass
        {
            Name "VFXPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _FRESNEL_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; 
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uvMain : TEXCOORD0;
                float2 uvNoise : TEXCOORD1;
                float4 color : COLOR;
                float3 normalWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _MainTex_ST;
                float4 _NoiseTex_ST;
                float _MainTexUSpeed;
                float _MainTexVSpeed;
                float _NoiseUSpeed;
                float _NoiseVSpeed;
                float _DistortionStrength;
                float _DissolveAmount;
                float _DissolveEdgeWidth;
                float4 _DissolveEdgeColor;
                float _FresnelPower;
                float4 _FresnelColor;
            CBUFFER_END

            TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex);   SAMPLER(sampler_NoiseTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;

                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);

                float2 mainPan = _Time.y * float2(_MainTexUSpeed, _MainTexVSpeed);
                float2 noisePan = _Time.y * float2(_NoiseUSpeed, _NoiseVSpeed);

                output.uvMain = TRANSFORM_TEX(input.uv, _MainTex) + mainPan;
                output.uvNoise = TRANSFORM_TEX(input.uv, _NoiseTex) + noisePan;
                output.color = input.color * _BaseColor;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half noiseValue = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.uvNoise).r;
                
                // UV Distortion
                float2 distortedUV = input.uvMain + (noiseValue * _DistortionStrength * 0.1);
                
                half4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
                half4 finalColor = mainCol * input.color;

                // Dissolve Logic
                half dissolveMask = noiseValue - _DissolveAmount;
                clip(dissolveMask);

                // Edge Glow
                half edgeFactor = smoothstep(0, _DissolveEdgeWidth, dissolveMask);
                finalColor.rgb += _DissolveEdgeColor.rgb * (1.0 - edgeFactor) * finalColor.a;

                // Fresnel Effect
                #if _FRESNEL_ON
                    float NdotV = saturate(dot(input.normalWS, input.viewDirWS));
                    float fresnel = pow(1.0 - NdotV, _FresnelPower);
                    finalColor.rgb += fresnel * _FresnelColor.rgb * _FresnelColor.a;
                #endif

                finalColor.a *= edgeFactor;

                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}