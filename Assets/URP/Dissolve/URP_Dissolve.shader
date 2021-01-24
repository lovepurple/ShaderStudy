Shader "URP/URP_Dissolve" 
{
    Properties 
    {
        _BaseTex ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _DissolveNoiseTex ("Dissolve Texture",2D) = "white"{}
        [HDR]_DissolveEdgeColor ("Dissolve Edge Color", Color) = (0,0.8,0,1)
        _DissolveRange ("Dissolve Range", Range(0,1) ) = 0
        _DissolveEdge("Dissolve Edge",Range(0,1))= 0.1
    }
    SubShader 
    {
        Tags 
        {
            "IgnoreProjector"="True"
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
            "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
        #include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
        #include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
        ENDHLSL

        Pass 
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            TEXTURE2D(_BaseTex);
            SAMPLER(sampler_BaseTex);

            TEXTURE2D(_DissolveNoiseTex);
            SAMPLER(sampler_DissolveNoiseTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseTex_ST;
            float4 _DissolveNoiseTex_ST;
            float4 _DissolveEdgeColor;
            float _DissolveEdge;
            float _DissolveRange;
            float4 _BaseColor;

            CBUFFER_END

            struct a2v 
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f 
            {
                float3 positionWS:TEXCOORD0 ;
                float4 uv:TEXCOORD1;
                float4 positionCS:SV_POSITION;
            };

            v2f vert (a2v i) 
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
                o.uv.xy = TRANSFORM_TEX(i.uv,_BaseTex);
                o.uv.zw = TRANSFORM_TEX(i.uv,_DissolveNoiseTex);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET 
            {
                float3 baseTexColor = SAMPLE_TEXTURE2D(_BaseTex,sampler_BaseTex,i.uv.xy).rgb;
                float3 baseColor = baseTexColor * _BaseColor.rgb;

                float dissolveFactor = SAMPLE_TEXTURE2D(_DissolveNoiseTex,sampler_DissolveNoiseTex,i.uv.zw).r;
                clip(dissolveFactor - _DissolveRange-0.01);

                // alpha的差 <= edge 就是edge的颜色
                float edgeStep = step(dissolveFactor-_DissolveRange,_DissolveEdge);
                float3 color = lerp(baseColor,_DissolveEdgeColor,edgeStep);
                
                return float4(color,1.0f);
            }
            ENDHLSL
        }
    }
}
