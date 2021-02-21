Shader "URP/URP_ExpolsionRipple" 
{
	Properties
	{
		_RippleNormalMap("Ripple Normal Map", 2D) = "bump" {}
		_ExpolsionSpeed("Expolsion Speed",Float) =1
	}
	SubShader
	{
		Tags
		{ 
			"RenderType" = "Transparent" 
			"RenderPipeline" = "UniversalPipeline"
			"Queue"="Transparent"
		}

		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"

		TEXTURE2D(_RippleNormalMap);
		SAMPLER(sampler_RippleNormalMap);

		TEXTURE2D(_CameraOpaqueTexture);
		SAMPLER(sampler_CameraOpaqueTexture);

		struct a2v
		{
			float4 positionOS:POSITION;
			float2 uv:TEXCOORD0;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float2 uv:TEXCOORD0;
			float2 positionSS:TEXCOORD1;
		};

		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert(a2v v)
			{
				v2f o =(v2f)0;

				return o;
			}

			float4 frag(v2f i):SV_TARGET
			{
				return float4(1,1,1,1);
			}

			ENDHLSL

			
		}
	}
}
