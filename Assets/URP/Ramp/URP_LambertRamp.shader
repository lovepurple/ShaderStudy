/**
Ramp的过渡使用贴图映射
*/
Shader "URP/URP_LambertRamp" {
	Properties 
	{
		_BaseTex ("Base Texture", 2D) ="white"{}
		_BaseColor("Base Color",Color)=(1,1,1,1)

		_NormalTex ("Normal Texture", 2D) = "bump"{}

		_RampTex ("Ramp Texture", 2D) = "white" {}
	}

	SubShader 
	{ 
		Tags 
		{
			"RenderType"="Opaque" 
			"Queue"= "Geometry"
			"RenderPipeline" = "UniversalPipeline"
		}
		Cull Back

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
		ENDHLSL
		
		Pass
		{
			HLSLPROGRAM
			TEXTURE2D(_BaseTex);
			SAMPLER(sampler_BaseTex);

			TEXTURE2D(_NormalTex);
			SAMPLER(sampler_NormalTex);

			TEXTURE2D(_RampTex);
			SAMPLER(sampler_RampTex);

			CBUFFER_START(UnityPerMaterial)
			float4 _BaseColor;
			float4 _BaseTEX_ST;
			float4 _NormalTex_ST;
			CBUFFER_END

			
			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{

			};

			struct v2f
			{

			};

			v2f vert(a2v i)
			{

			}

			float4 frag(v2f i):SV_TARGET2
			{

			}
			

			ENDHLSL

		}
	}
}
