/*
深度 深度图采样 及 屏幕空间 
*/

Shader "URP/URP_Shield"
{
	Properties
	{
		_MainTex("Main Texture ", 2D) = "white" {}
		_BaseColor("Base Color",Color) = (1,1,1,1)
		_IntersectionColor("Intersection Color",Color) = (1,0.2,0,1)
		_IntersectionEdge("Intersection Edge",Range(0,0.1)) = 0.01
		_FlowSpeed("Flow Speed",Float) =1.0
		_FlowColor("Flow Color",Color) = (0,0,0.8,1)
		_FlowRange("Flow Range",Float) = 1
		_FlowSteamerSpacing("Flow Steamer Spacing",Range(0,1)) = 0.2
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
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\Common.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Lighting.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Shadows.hlsl"

		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);

		TEXTURE2D(_CameraDepthTexture);
		SAMPLER(sampler_CameraDepthTexture);

		CBUFFER_START(UnityPerMaterial)
		float4 _BaseColor;
		float4 _IntersectionColor;
		float _IntersectionEdge;
		float4 _FlowColor;
		float _FlowSpeed;
		float _FlowRange;
		float _FlowSteamerSpacing;
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
			float4 positionSS:TEXCOORD5;

		};
		
		ENDHLSL

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
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
				o.positionSS = ComputeScreenPos(o.positionCS / o.positionCS.w);
				o.positionSS.zw = o.positionCS.zw;				//w 是depth

				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);
				
				float3 depthCol = SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_CameraDepthTexture,i.positionSS.xy);
				float depth =  Linear01Depth(i.positionCS.z,_ZBufferParams);			//SV_Position的 z是深度 (0-1)
				float screenDepth = Linear01Depth(depthCol.r,_ZBufferParams);

				float isIntersectionEdge =step(abs(screenDepth - depth) * 10,_IntersectionEdge);
				isIntersectionEdge*= (1-abs(screenDepth - depth) * 100);		//边缘软过渡
				
				float flow = pow(1-abs(frac(i.positionWS.y * _FlowSteamerSpacing - _Time.y * _FlowSpeed) - 0.5f),_FlowRange);

				float4 flowColor = _FlowColor * flow;

				float4 mainColor =float4( _BaseColor.rgb + flowColor.rgb,_BaseColor.a);

				float4 col = lerp(mainColor,_IntersectionColor,isIntersectionEdge);
				
				return col;
			}
			ENDHLSL
		}
	}
}
