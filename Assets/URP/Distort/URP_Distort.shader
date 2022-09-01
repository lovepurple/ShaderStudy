/*
    2022-09-01 15:17:22
    根据一张灰度通道扭曲UV
*/
Shader "URP/Distort"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _DistortTex ("Distort Texture", 2D) = "white" { }
        _DistortAmount ("Distort Amount", Range(0, 2)) = 0
        _Speed ("Animation Speed", Range(0, 2)) = 0
    }


    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Opaque" "RenderType" = "Opaque" }
        
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        TEXTURE2D(_DistortTex);
        SAMPLER(sampler_DistortTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _DistortTex_ST;
            float _DistortAmount;
            float _Speed;
        CBUFFER_END

        struct a2v
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };
        
        ENDHLSL
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            v2f vert(a2v a)
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
                o.uv = TRANSFORM_TEX(a.uv, _DistortTex);

                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float2 distortUV = SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex, i.uv).rg + _Time.y * _Speed ;
                float2 uv = lerp(i.uv, i.uv + distortUV, _DistortAmount);
                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                return baseColor;
            }

            ENDHLSL
        }
    }
}