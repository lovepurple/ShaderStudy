Shader "URP/URP_WorldNormal"
{
	Properties
	{
		_BaseMap("Base Texture",2D)="white"{}
		_BaseColor("Base Color",Color) = (1,1,1,1)
		_NormalTex("Normal Texture",2D)="bump"{}
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

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			TEXTURE2D(_NormalTex);
			SAMPLER(sampler_NormalTex);
			
			CBUFFER_START(UnityPerMaterial)
			float4 _BaseColor;
			float4 _BaseMap_ST;
			CBUFFER_END
			struct a2v
			{
				float2 uv:TEXCOORD0;
				float4 positionOS:POSITION;
				float3 normalOS:NORMAL;
				float4 tangentOS:TANGENT;
			};

			struct v2f
			{
				float3 normalWS:TEXCOORD0;
				float3 tangentWS:TEXCOORD2;
				float2 uv:TEXCOOR1;
				float4 positionCS:SV_POSITION;
			};

			v2f vert (a2v v)
			{
				v2f output = (v2f)0;
				VertexPositionInputs vertexOutput = GetVertexPositionInputs(v.positionOS.xyz);
				output.positionCS = vertexOutput.positionCS;
				output.uv = TRANSFORM_TEX(v.uv,_BaseMap);
				output.normalWS = TransformObjectToWorldNormal(v.normalOS,true);
				output.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz,true) * v.tangentOS.w;
				
				return output;
			}

			float4 frag (v2f i) : SV_Target
			{	
				float3 bitangentWS = cross(i.tangentWS,i.normalWS);
				float3x3 tbnMatrix = float3x3(
				float3(i.tangentWS.x,bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,bitangentWS.z,i.normalWS.z)
				);

				float4 normalCol = SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv);
				float3 normalTex = UnpackNormal(float4(normalCol.r,normalCol.g, 1- normalCol.b,normalCol.a));

				float3 normalDir = normalize(mul(tbnMatrix,normalTex));
				
				return float4(normalDir,1.0f);
				
			}
			ENDHLSL
		}
	}
}