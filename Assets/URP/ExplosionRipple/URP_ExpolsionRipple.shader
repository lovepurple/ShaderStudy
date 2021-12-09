Shader "URP/URP_ExpolsionRipple" 
{
	Properties
	{
		_RippleNormalMap("Ripple Normal Map", 2D) = "bump" {}
		_ExpolsionSpeed("Expolsion Speed",Float) =1
		_RippleNormalScale("Ripple Power Scale ",Float) = 1
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

		CBUFFER_START(UnityPerMaterial)
		float _RippleNormalScale;
		float _ExpolsionSpeed;
		CBUFFER_END

		struct a2v
		{
			float4 positionOS:POSITION;
			float2 uv:TEXCOORD0;
			float4 tangentOS:TANGENT;
			float3 normalOS:NORMAL;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float2 uv:TEXCOORD0;
			float2 positionSS:TEXCOORD1;
			float3 normalWS:TEXCOORD2;
			float3 bitangentWS:TEXCOORD3;
			float3 tangentWS:TEXCOORD4;
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
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				o.positionSS = ComputeScreenPos(o.positionCS / o.positionCS.w).xy ;
				o.uv = v.uv;
				o.normalWS = TransformObjectToWorldNormal(v.normalOS);
				o.tangentWS = normalize(TransformObjectToWorldDir(v.tangentOS.xyz) * v.tangentOS.w);
				o.bitangentWS = normalize(cross(o.normalWS,o.tangentWS));

				return o;
			}

			float4 frag(v2f i):SV_TARGET
			{
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);

				float3 normalDir = UnpackNormal(SAMPLE_TEXTURE2D(_RippleNormalMap,sampler_RippleNormalMap,i.uv ));
				normalDir.xy *= _RippleNormalScale;
				normalDir.z = sqrt( 1 - normalDir.x * normalDir.x - normalDir.y * normalDir.y);
				float3 normalWS = normalize(mul(tbn,normalDir));

				float3 baseColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture,sampler_CameraOpaqueTexture,i.positionSS + normalWS.xy);


				return float4(baseColor,1);
			}

			ENDHLSL

			
		}
	}
}
