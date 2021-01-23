Shader "URP/URP_Transparent" 
{
    Properties 
    {
        _BaseTex("Base Texture",2D)="white"{}
        _BaseColor("Base Color",Color) = (1,1,1,1)

        _AlphaTex("AlphaChannel(R)",2D)="white"{}
    }
    SubShader 
    {
        Tags 
        {
            "RenderType" = "Transparent"
            "Queue"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "LightMode"="UniversalForward"
            "IgnoreProjector" = "True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest On

        HLSLINCLUDE
        #include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
        #include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
        #include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Lighting.hlsl"
        #include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
        ENDHLSL

        Pass 
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            TEXTURE2D(_BaseTex);
            SAMPLER(sampler_BaseTex);

            TEXTURE2D(_AlphaTex);
            SAMPLER(sampler_AlphaTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseTex_ST;
            float4 _BaseColor;
            float4 _AlphaTex_ST;
            CBUFFER_END

            struct a2v 
            {
                float4 positionOS: POSITION;
                float3 normalOS:NORMAL;
                float2 uv:TEXCOORD0;
            };
            
            struct v2f 
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS:TEXCOORD0;
                float4 uv:TEXCOORD1;
            };
            
            v2f vert (a2v i) 
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(i.normalOS);
                o.uv.xy = TRANSFORM_TEX(o.uv,_BaseTex);
                o.uv.zw = TRANSFORM_TEX(o.uv,_AlphaTex);

                return o;
            }

            float4 frag(v2f i) : SV_TARGET 
            {
                Light mainLightInfo = GetMainLight();
                float NDL = max(0,dot(i.normalWS,mainLightInfo.direction));
                float3 baseColor = SAMPLE_TEXTURE2D(_BaseTex,sampler_BaseTex,i.uv.xy);
                baseColor *= _BaseColor * mainLightInfo.color * (NDL * 0.5f +0.5f);
                float alpha = SAMPLE_TEXTURE2D(_AlphaTex,sampler_AlphaTex,i.uv.zw).r;

                return float4(baseColor,alpha);
            }
            ENDHLSL
        }
    }
}
