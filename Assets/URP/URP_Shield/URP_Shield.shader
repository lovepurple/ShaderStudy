/*
深度的使用 及 屏幕空间 
*/

Shader "URP/URP_Shield"
{
	Properties
	{
		_MainTex("Main Texture ", 2D) = "white" {}
		_BaseColor("Base Color",Color) = (1,1,1,1)

	}
	SubShader
	{
		Tags 
		{ 
			"RenderType"="Transparent"
			"RenderPipeline" = "UnversalPipeline"
			"LightMode" = "UniversalForward"
			"Queue" = "Transparent"
		}

		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Lighting.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Shadows.hlsl"

		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);

		CBUFFER_START(UnityPerMaterial)
		float4 _BaseColor;
		CBUFFER_END

		struct a2v
		{
			float4 positionOS:POSITION;
			float3 normalOS:NORMAL;
			float4 tangentOS:TANGENT;
			float2 uv:TEXCOORD0;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float3 normalWS:TEXCOORD0;
			float3 tangentWS:TEXCOORD1;
			float3 positionWS:TEXCOORD2;
			float3 bitangentWS:TEXCOORD3;
			float2 uv:TEXCOORD4;

		};
		
		ENDHLSL

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag


			v2f vert (a2v a)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
				o.normalWS = TransformObjectToWorldNormal(a.normalOS);
				o.tangentWS = TransformObjectToWorldDir(a.tangentOS.xyz) * a.tangentOS.w;
				o.bitangentWS = cross(o.normalWS,o.tangentWS);
				o.uv = a.uv;
				o.positionWS = TransformObjectToWorld(a.positionOS.xyz);

				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);
				
				float2 screenUV = i.positionCS.xy / _ScreenParams.xy;

				return float4(screenUV,0,1.0);
				
				
			}
			ENDHLSL
		}
	}
}
