Shader "URP/URP_VertexColor"
{
	Properties
	{
	}

	SubShader
	{
		Tags{ 
			"RenderType" = "Opaque"  
			"RenderPipeline"="UniversalPipeline"
			"LightMode"="UniversalForward"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			CBUFFER_START(UnityPerMaterial)
			CBUFFER_END
			struct a2v
			{
				float4 positionOS:POSITION;
				float4 color:COLOR;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 color:TEXCOORD3;
				float4 positionCS:SV_POSITION;
				float2 uv:TESSFACTOR0;
			};

			v2f vert (a2v v)
			{
				v2f output = (v2f)0;
				VertexPositionInputs vertexOutput = GetVertexPositionInputs(v.positionOS.xyz);
				output.positionCS = vertexOutput.positionCS;
				output.color = v.color;
				output.uv = v.uv;
				
				return output;
			}

			float4 frag(v2f i) : SV_Target
			{				
				return float4(i.color.rgb,1.0);
				
			}
			ENDHLSL
		}
	}
}