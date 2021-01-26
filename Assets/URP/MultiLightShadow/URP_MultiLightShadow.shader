/**
多光源的阴影处理
*/
Shader "URP/URP_MultiLightShadow" 
{
	Properties 
	{
		_AlbedoTex("Albedo Texture", 2D) = "white" {}		
		_BaseColor("Base Color", Color) = (1,1,1,1)

		_NormalTex ("Normal Map", 2D) = "bump" {}
		[KeywordEnum(ON,OFF)]_UseMultiLight("Use MultiLight",Float) = 1
	}

	SubShader 
	{ 
		Tags 
		{
			"RenderType"="Opaque" 
			"Queue"= "Geometry"
			"RenderPipeline" = "UniversalPipeline"
			"LightMode" = "UniversalForward"	
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
		ENDHLSL
		
		Pass
		{
			HLSLPROGRAM

			#pragma shader_feature _USEMULTILIGHT_ON _USEMULTILIGHT_OFF
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			//额外光源的阴影 定义宏
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

			#pragma vertex vert
			#pragma fragment frag

			TEXTURE2D(_AlbedoTex);
			SAMPLER(sampler_AbledoTex);

			TEXTURE2D(_NormalTex);
			SAMPLER(sampler_NormalTex);
			
			CBUFFER_START(UnityPerMaterial)
			float4 _BaseColor;
			float _UseMultiLight;
			CBUFFER_END

			struct a2v
			{
				float3 positionOS:POSITION;
				float3 normalOS:NORMAL;
				float4 tangentOS:TANGENT;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float2 uv:TEXCOORD0;
				float4 positionCS:SV_POSITION;
				float3 positionWS:TEXCOORD1;
				float3 tangentWS:TEXCOORD2;
				float3 bitangentWS:TEXCOORD3;
				float3 normalWS:TEXCOORD4;
			};

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
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);

				float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv)).rgb;
				float3 normalDir = normalize(mul(tbn,normalTex));

				return float4(normalDir,1.0);

			}
			ENDHLSL
		}
		UsePass "Universal Render Pipeline/Lit/ShadowCaster"		//物体写入Shadowmap
	}
}
