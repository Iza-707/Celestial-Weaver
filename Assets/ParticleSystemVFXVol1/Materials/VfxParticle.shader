// VfxParticle.shader - the shader the packs' particles draw with.
//
// Replaces Legacy Shaders/Particles/Additive and Alpha Blended, for two
// reasons.
//
// The first is that those two do not exist outside the Built-in pipeline. A
// buyer on URP or HDRP currently opens the materials and switches them by
// hand, which the listings have to explain and which is exactly the kind of
// homework the packs selling for five times as much do not set. This uses only
// UnityCG.cginc and UnityObjectToClipPos, which every pipeline ships, and its
// pass carries no LightMode tag, so URP and HDRP draw it as unlit rather than
// substituting the magenta error shader.
//
// The second is erosion. A particle that fades out uniformly reads as a
// picture being turned down; a particle that dissolves through a noise field
// reads as smoke thinning and fire burning out. The threshold is driven by the
// particle's own alpha, so it needs no custom vertex streams and no per-effect
// setup - the colour-over-lifetime curves the pack already authors drive it.
//
// One shader, both blend modes: the builder sets _SrcBlend and _DstBlend on
// the material rather than picking a different shader.
Shader "Taproot/VFX Particle"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _TintColor ("Tint", Color) = (0.5, 0.5, 0.5, 0.5)
        _Boost ("Intensity", Range(0.5, 4)) = 2.0

        _ErosionTex ("Erosion (R)", 2D) = "black" {}
        _Erosion ("Erosion Amount", Range(0, 1)) = 0.0
        _ErosionScale ("Erosion Scale", Range(0.25, 4)) = 1.0
        _SoftEdge ("Erosion Softness", Range(0.01, 0.5)) = 0.15

        // Set by the builder per material; also what lets one shader serve
        // both the additive and the alpha blended materials.
        [HideInInspector] _SrcBlend ("src", Float) = 5   // SrcAlpha
        [HideInInspector] _DstBlend ("dst", Float) = 1   // One
    }

    Category
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend [_SrcBlend] [_DstBlend]
        ColorMask RGB
        Cull Off
        Lighting Off
        ZWrite Off

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_particles
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                sampler2D _ErosionTex;
                fixed4 _TintColor;
                float4 _MainTex_ST;
                float _Boost, _Erosion, _ErosionScale, _SoftEdge;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                v2f vert (appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_OUTPUT(v2f, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.texcoord);
                    fixed4 col = _Boost * i.color * _TintColor * tex;

                    // Dissolve rather than fade. As the particle's own alpha
                    // falls, the threshold rises through the noise, so the
                    // sprite breaks up from its thinnest parts first. At
                    // _Erosion 0 this is exactly a fade, which keeps the
                    // effects that want one unchanged.
                    if (_Erosion > 0.0)
                    {
                        float n = tex2D(_ErosionTex, i.texcoord * _ErosionScale).r;
                        float life = i.color.a;
                        float threshold = (1.0 - life) * _Erosion;
                        col.a *= smoothstep(threshold, threshold + _SoftEdge, n);
                    }

                    return col;
                }
                ENDCG
            }
        }
    }
}
