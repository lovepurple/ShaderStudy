/**
    基于URP实现的边缘光
*/
Shader "URP/URP_EdgeRim"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _NormalTex ("Normal Texture", 2D) = "bump" { }
        _RimRange ("Rim Range", Range(0.001, 1)) = 0.2
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "IgnoreProjector" = "True" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        //包含一些ViewDir的计算
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        TEXTURE2D(_NormalTex);
        SAMPLER(sampler_NormalTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _RimRange;
            float4 _RimColor;
        CBUFFER_END

        struct a2v
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
            float3 targentWS : TEXCOORD2;
            float3 positionWS : TEXCOORD3;
        };

        ENDHLSL

        Pass
        {
            //Tags 写在Pass里 Shader是紫红色的 还不报编译错误
            // Tags { "LightMode" = "ForwardBase" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            v2f vert(a2v a)
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(a.positionOS.xyz);
                o.uv = TRANSFORM_TEX(a.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(a.normalOS);
                o.targentWS = TransformObjectToWorldDir(a.tangentOS.xyz) * a.tangentOS.w;

                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                Light mainLightData = GetMainLight();
                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(i.positionWS);

                float3 bitangentWS = cross(normalize(i.normalWS), normalize(i.targentWS));
                float3x3 tbn_T = float3x3(normalize(i.targentWS), bitangentWS, normalize(i.normalWS));
                
                float3 normalCol = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.uv));
                float3 normalWS = mul(transpose(tbn_T), normalCol);
                float NDV = dot(normalWS, viewDirWS);
                float edgeRange = max(0.0001, saturate(1 - NDV));           //0的0次方等于1
                
                float NDL = max(0, dot(normalWS, mainLightData.direction));
                float3 edgeRimColor = pow(edgeRange, _RimRange * 10) * _RimColor * NDL;

                float halfLambert = 0.5 + 0.5 * NDL;
                float3 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb * mainLightData.color;
                
                float4 finalColor = float4(baseColor, 1.0) + float4(edgeRimColor, 1.0);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}
