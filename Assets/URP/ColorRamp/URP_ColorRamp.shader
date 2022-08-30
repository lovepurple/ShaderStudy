/**
    根据HalfLamber 计算出明暗色阶
*/
Shader "URP/ColorRamp"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" { }
        _NormalTex ("_NormalTex", 2D) = "bump" { }
        _StepCount ("_StepCount", float) = 1

        
        [Toggle(USE_TEXTURE)]
        _UseRampTexture ("Use RampTexture", Float) = 0
        //使用Ramp过渡贴图
        _RampTex ("_RampTex(R)", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        TEXTURE2D(_NormalTex);
        SAMPLER(sampler_NormalTex);

        TEXTURE2D(_RampTex);
        SAMPLER(sampler_RampTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _StepCount;
        CBUFFER_END

        struct appdata
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
        };

        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normalWS : TEXCOORD1;
            float3 tangentWS : TEXCOORD2;
        };
        

        ENDHLSL

        Pass
        {
            HLSLPROGRAM

            #pragma multi_compile _ USE_TEXTURE
            #pragma vertex vert
            #pragma fragment frag
            
            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS, true);
                o.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz, true) * v.tangentOS.w;
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                float3 bitarngent = mul(i.normalWS, i.tangentWS);
                float3x3 tbn_t = float3x3(normalize(i.tangentWS), normalize(bitarngent), normalize(i.normalWS));
                float3x3 tbn = transpose(tbn_t);

                float3 normalCol = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.uv));
                float3 normalDirWS = mul(tbn, normalCol);
                Light mainLightData = GetMainLight();
                float NDL = dot(normalDirWS, mainLightData.direction);
                float halfLampert = 0.5 + 0.5 * NDL;

                #ifdef USE_TEXTURE
                    float stepColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, half2(1 - halfLampert, 0.5)).r;
                #else
                    //色阶方法
                    float stepColor = lerp(halfLampert, floor(halfLampert * _StepCount) / _StepCount, step(0.001, _StepCount));
                #endif

                float3 finalCol = baseColor.rgb * mainLightData.color * stepColor;
                

                return float4(finalCol, 1.0);
            }
            
            ENDHLSL
        }
    }
}