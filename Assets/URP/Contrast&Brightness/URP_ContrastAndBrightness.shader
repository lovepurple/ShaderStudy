/**
    调节对比度和明亮度的算法
    col.rgb  = (base.rgb - 0.5 * _contrast + 0.5 +_brightness)
    对比度和明度要一起用，单调对比度会更黑，需要加上明度把颜色调整回来
*/
Shader "URP/ContrastAndBrightness"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _NormalTex ("Normal Texture", 2D) = "bump" { }
        
        //对比度调节
        _Contrast ("Contrast", Range(0, 5)) = 1

        //明亮度调节
        _Brightness ("Brightness", Range(-1, 1)) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "Opaque" "RenderType" = "Opaque" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        TEXTURE2D(_NormalTex);
        SAMPLER(sampler_NormalTex);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _Contrast;
            float _Brightness;
        CBUFFER_END

        struct a2v
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
        };

        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float3 normalWS : TEXCOOR1;
            float3 tangentWS : TEXCOORD2;
            float2 uv : TEXCOORD;
        };

        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VERT
            #pragma fragment FRAG

            v2f VERT(a2v v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz) * v.tangentOS.w;
                return o;
            }

            float4 FRAG(v2f i) : SV_TARGET
            {
                float3 bitangentWS = cross(i.normalWS, i.tangentWS);
                float3x3 tbn_T = float3x3(normalize(i.tangentWS), normalize(bitangentWS), normalize(i.normalWS));
                float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.uv));
                float3 normalWS = mul(transpose(tbn_T), normalTex);

                Light mainLightData = GetMainLight();
                float NDL = max(0, dot(mainLightData.direction, normalWS));
                float halfLambert = 0.5 * NDL + 0.5;

                float3 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * halfLambert;

                //调整对比度和亮度
                baseColor = max(0, (baseColor - 0.5) * _Contrast + 0.5 + _Brightness) * mainLightData.color;

                return float4(baseColor, 1.0);
            }

            ENDHLSL
        }
    }
}