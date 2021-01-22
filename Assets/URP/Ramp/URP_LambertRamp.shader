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
			"LightMode"="UniversalForward"
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
			float4 _BaseTex_ST;
			float4 _NormalTex_ST;
			CBUFFER_END

			
			#pragma vertex vert
			#pragma fragment frag

			struct a2v
			{
				float4 positionOS:POSITION;
				float3 normalOS:NORMAL;
				float2 uv:TEXCOORD;
				float4 tangentOS:TANGENT;
			};

			struct v2f
			{
				float3 positionWS:TEXCOORD;
				float2 uv:TEXCOORD1;
				float3 normalWS:TEXCOORD2;
				float4 positionCS:SV_POSITION;
				float3 tangentWS:TEXCOORD3;
			};

			v2f vert(a2v i)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
				o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
				o.normalWS = TransformObjectToWorldNormal(i.normalOS);
				o.tangentWS = TransformObjectToWorldDir(i.tangentOS.xyz) * i.tangentOS.w;
				o.uv = TRANSFORM_TEX(i.uv,_BaseTex);

				return o;
			}

			float4 frag(v2f i):SV_TARGET
			{
				float3 bitangent = cross(i.normalWS,i.tangentWS);
				float3x3 tbn_IT = float3x3(
				i.tangentWS,bitangent,i.normalWS
				);

				float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv)).xyz;
				float3 normalDir = normalize(mul(normalTex,tbn_IT));

				Light mainLightInfo = GetMainLight();
				float NDL = dot(normalDir,mainLightInfo.direction);
			
				float HNDL = NDL * 0.5f + 0.5f;

				float3 rampColor = SAMPLE_TEXTURE2D(_RampTex,sampler_RampTex,float2(HNDL,HNDL)).rgb;
				float3 baseColor = SAMPLE_TEXTURE2D(_BaseTex,sampler_BaseTex,i.uv).rgb;
				
				float3 color = baseColor * mainLightInfo.color * rampColor.r *_BaseColor.rgb;

				return float4(color.rgb,1.0);
			}
			

			ENDHLSL

		}
	}
}
