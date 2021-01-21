Shader "URP/URP_Unlit"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor("BaseColor",Color) = (1,1,1,1)
        _CutOff("AlphaCutOff",Range(0.0,1.0)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag

            struct a2v
            {
                float4 positionOS : POSITION;
                // float3 normalOS:NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                half _CutOff;
            CBUFFER_END


            v2f vert (a2v v)
            {
                v2f output = (v2f)0;
                VertexPositionInputs vertexOutput = GetVertexPositionInputs(v.positionOS.xyz);
                output.vertex = vertexOutput.positionCS;
                output.uv = TRANSFORM_TEX(v.uv,_BaseMap);
                return output;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv);
                float3 color = texColor.rgb * _BaseColor.rgb;

                clip(_BaseColor.a - _CutOff);

                return float4(color.rgb,1.0f);
                
            }
            ENDHLSL
        }
    }
}
