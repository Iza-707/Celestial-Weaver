Shader "StylizedFX/Lit_Optimized"
{
    Properties
    {
        [Header(Main Settings)]
        [HDR] [MainColor] _BaseColor("Base Color (HDR)", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Main Texture (RGBA)", 2D) = "white" {} 
        _MainTexUSpeed("Main Tex U Speed", Float) = 0
        _MainTexVSpeed("Main Tex V Speed", Float) = 0

        [NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}

        [Header(Lighting Settings)]
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _SpecularColor("Specular Color", Color) = (0.2, 0.2, 0.2, 1)
        [Toggle(_RAMP_ON)] _UseRamp("Use Toon Ramp", Float) = 0
        _RampThreshold("Ramp Threshold", Range(0, 1)) = 0.5
        _RampSmoothness("Ramp Smoothing", Range(0.001, 1)) = 0.1

        [Header(Noise Settings)]
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NoiseUSpeed("Noise U Speed", Float) = 0.2
        _NoiseVSpeed("Noise V Speed", Float) = 0.1
        
        [Header(Distortion)]
        _DistortionStrength("Distortion Strength", Range(0, 1)) = 0.1
        _DissolveAmount("Dissolve Amount", Range(0, 1.01)) = 0
        _DissolveEdgeWidth("Dissolve Edge Width", Range(0.01, 0.5)) = 0.1
        [HDR] _DissolveEdgeColor("Dissolve Edge Color", Color) = (2,1,0,1)

        [Header(Fresnel Settings)]
        _FresnelPower("Fresnel Power", Range(0.1, 10)) = 2
        [HDR] _FresnelColor("Fresnel Color", Color) = (1,1,1,1)

        [Header(Intersection(Edge) Softness)]
        [Toggle(_DEPTHFADE_ON)] _UseDepthFade("Enable Soft Intersection", Float) = 0
        _DepthFadeDistance("Softness Distance", Range(0, 5)) = 1.0

        // Blending & Culling // Render State
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1 
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dest Blend", Float) = 0 
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2 
        [Toggle(_ZWRITE_ON)] _ZWrite("ZWrite", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Blend [_SrcBlend] [_DstBlend]
        Cull [_Cull]
        ZWrite [_ZWrite]

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _MainTex_ST;
            float _MainTexUSpeed;
            float _MainTexVSpeed;
            
            float _Smoothness;
            float4 _SpecularColor;
            float _UseRamp;
            float _RampThreshold;
            float _RampSmoothness;

            float4 _NoiseTex_ST;
            float _NoiseUSpeed;
            float _NoiseVSpeed;
            
            float _DistortionStrength;
            float _DissolveAmount;
            float _DissolveEdgeWidth;
            float4 _DissolveEdgeColor;
            
            float _FresnelPower;
            float4 _FresnelColor;

            float _SrcBlend;
            float _DstBlend;
            float _Cull;
            float _ZWrite; 

            float _UseDepthFade;
            float _DepthFadeDistance;
        CBUFFER_END

        TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);
        TEXTURE2D(_NoiseTex);   SAMPLER(sampler_NoiseTex);
        TEXTURE2D(_BumpMap);    SAMPLER(sampler_BumpMap);
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _RAMP_ON
            #pragma shader_feature_local _DEPTHFADE_ON
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uvMain : TEXCOORD0;
                float2 uvNoise : TEXCOORD1;
                float4 color : COLOR;
                float3 normalWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5; 
                float3 tangentWS : TEXCOORD6;
                float3 bitangentWS : TEXCOORD7;
                float fogFactor : TEXCOORD8;
                float4 screenPos : TEXCOORD9;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.shadowCoord = GetShadowCoord(vertexInput);

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;
                
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);

                float2 mainPan = _Time.y * float2(_MainTexUSpeed, _MainTexVSpeed);
                float2 noisePan = _Time.y * float2(_NoiseUSpeed, _NoiseVSpeed);

                output.uvMain = TRANSFORM_TEX(input.uv, _MainTex) + mainPan;
                output.uvNoise = TRANSFORM_TEX(input.uv, _NoiseTex) + noisePan;
                output.color = input.color * _BaseColor;
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                output.screenPos = ComputeScreenPos(vertexInput.positionCS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half noiseValue = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.uvNoise).r;
                float2 distortedUV = input.uvMain + (noiseValue * _DistortionStrength * 0.1);

                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV) * input.color;
                half dissolveMask = noiseValue - _DissolveAmount;
                clip(dissolveMask);
                
                half edgeFactor = smoothstep(0, _DissolveEdgeWidth, dissolveMask);
                float3 emission = _DissolveEdgeColor.rgb * (1.0 - edgeFactor) * albedo.a;

                half4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, distortedUV);
                float3 normalTS = UnpackNormal(normalMap);
                float3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
                normalWS = normalize(normalWS);

                Light mainLight = GetMainLight(input.shadowCoord);
                
                float NdotL = dot(normalWS, mainLight.direction);
                #if _RAMP_ON
                    float ramp = smoothstep(_RampThreshold - _RampSmoothness, _RampThreshold + _RampSmoothness, NdotL);
                    NdotL = ramp;
                #else
                    NdotL = saturate(NdotL);
                #endif

                float3 diffuse = albedo.rgb * mainLight.color * NdotL * mainLight.shadowAttenuation;
                float3 ambient = SampleSH(normalWS) * albedo.rgb;

                float3 halfVector = normalize(mainLight.direction + input.viewDirWS);
                float NdotH = saturate(dot(normalWS, halfVector));
                float specularIntensity = pow(NdotH, _Smoothness * 100.0);
                
                #if _RAMP_ON
                     specularIntensity = smoothstep(0.5, 0.51, specularIntensity);
                #endif
                
                float3 specular = _SpecularColor.rgb * specularIntensity * mainLight.shadowAttenuation;
                float NdotV = saturate(dot(normalWS, input.viewDirWS));
                float fresnel = pow(1.0 - NdotV, _FresnelPower);
                float3 fresnelColor = fresnel * _FresnelColor.rgb * mainLight.shadowAttenuation;
                
                float3 finalColor = diffuse + ambient + specular + fresnelColor + emission;
                finalColor = MixFog(finalColor, input.fogFactor);

                float fade = 1.0;
                #if _DEPTHFADE_ON
                    float2 screenUV = input.screenPos.xy / input.screenPos.w;
                    float rawDepth = SampleSceneDepth(screenUV);
                    float sceneDepth = LinearEyeDepth(rawDepth, _ZBufferParams); 
                    float partDepth = LinearEyeDepth(input.positionCS.z, _ZBufferParams); 
                    float fadeDistance = max(0.0001, _DepthFadeDistance);
                    fade = saturate((sceneDepth - partDepth) / fadeDistance);
                #endif 
                
                return half4(finalColor, albedo.a * fade);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 _LightDirection;

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                float3 positionWS = vertexInput.positionWS;
 
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                float3 normalWS = normalInput.normalWS; 
                float3 lightDirection = _MainLightPosition.xyz;
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirection));
                
                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif

                output.positionCS = positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _NoiseTex); 
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half noiseValue = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.uv).r;
                clip(noiseValue - _DissolveAmount);
                return 0;
            } 
            ENDHLSL
        } 
    } 
    FallBack "Universal Render Pipeline/Lit"
}