/**
实时阴影投射
*/

Shader "URP/URP_ShadowCaster" 
{
	Properties 
	{
		_AlbedoTex("Albedo Texture", 2D) = "white" {}
		_BaseColor("Base Color",Color) = (1,1,1,1)
		_NormalTex("Normal Texture",2D)="bump"{}
		_CutOffTex("CutOff Tex(R)",2D) = "white"{}
		[KeywordEnum(ON,OFF)] _UseShadow("Use Shadow",Float) = 1
	}

	SubShader 
	{
		Tags 
		{ 
			"RenderType"="Opaque"
			"RenderPipeline" = "UnversalPipeline"
			"Queue" = "AlphaTest"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
		#pragma multi_compile _ _SHADOWS_SOFT
		#pragma shader_feature _USESHADOW_ON _USESHADOW_OFF

		TEXTURE2D(_AlbedoTex);
		SAMPLER(sampler_AlbedoTex);

		TEXTURE2D(_NormalTex);
		SAMPLER(sampler_NormalTex);

		TEXTURE2D(_CutOffTex);
		SAMPLER(sampler_CutOffTex);

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
			float3 positionWS:TEXCOORD0;
			float3 tangentWS:TEXCOORD1;
			float3 bitangentWS:TEXCOORD2;
			float3 normalWS:TEXCOORD3;
			float2 uv:TEXCOORD4;
		};

		ENDHLSL
		
		//第一个Pass 正常渲染物体上色
		Pass
		{
			Tags
			{
				"LightMode" = "UniversalForward"
			}

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			v2f vert(a2v a)
			{
				v2f o = (v2f)0;
				o.uv = a.uv;
				o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
				o.positionWS = TransformObjectToWorld(a.positionOS.xyz);
				o.normalWS = TransformObjectToWorldNormal(a.normalOS);
				o.tangentWS = normalize(TransformObjectToWorldDir(a.tangentOS.xyz) * a.tangentOS.w);
				o.bitangentWS = normalize(cross(o.normalWS,o.tangentWS));

				return o;
			}


			float4 frag(v2f i):SV_TARGET
			{
				float2 positionSS = i.positionCS.xy / _ScreenParams.xy;
				float cutOffFactor = SAMPLE_TEXTURE2D(_CutOffTex,sampler_CutOffTex,positionSS).r;
				clip(cutOffFactor - 0.01f);

				float4 positionShadowCoord = TransformWorldToShadowCoord(i.positionWS);
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);

				float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv)).rgb;
				float3 normalWS = normalize(mul(tbn,normalTex));
				Light mainLightInfo = GetMainLight(positionShadowCoord);
				float3 baseCol = SAMPLE_TEXTURE2D(_AlbedoTex,sampler_AlbedoTex,i.uv) * _BaseColor;
				float NDL = dot(normalWS,normalize(mainLightInfo.direction));

				float3 col = baseCol * (NDL * 0.5 +0.5) * mainLightInfo.color;
				#if _USESHADOW_ON
					col *= mainLightInfo.shadowAttenuation;
				#endif

				return float4(col,1.0);
			}
			ENDHLSL
		}

		//第二个Pass阴影投射（写入当前物体的阴影信息到ShadowMap）
		Pass 
		{		
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			HLSLPROGRAM
			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster


			//positionCSShadowCaster 在灯光坐标系下的裁剪坐标系

			v2f vertShadowCaster(a2v a)
			{
				float3 positionWS = TransformObjectToWorld(a.positionOS.xyz);
				float3 normalWS = TransformObjectToWorldNormal(a.normalOS);
				Light mainLightInfo = GetMainLight();
				float3 lightDirWS = normalize(mainLightInfo.direction);
				float3 shadowCasterBiasWS = ApplyShadowBias(positionWS,normalWS,lightDirWS);		//返回的坐标已经加上了原始的positionWS
				//转换到正常的clip space 传入fragment
				v2f o = (v2f)0;
				o.positionCS = TransformWorldToHClip(shadowCasterBiasWS);
				o.uv = a.uv;

				return o;
			}

			float4 fragShadowCaster(v2f i):SV_TARGET
			{
				float2 positionSS = i.positionCS.xy / _ScreenParams.xy;
				float cutOffFactor = SAMPLE_TEXTURE2D(_CutOffTex,sampler_CutOffTex,positionSS).r;
				clip(cutOffFactor - 0.01f);
				return 0;

			}
			ENDHLSL
		}

	}	
}