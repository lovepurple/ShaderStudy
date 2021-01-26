/*
主光源的阴影计算流程

TransformWorldToShadowCoord(positionWS)
*/

Shader "URP/URP_MainLightShadow"
{
	Properties
	{
		_AlbedoTex("Albedo Texture ", 2D) = "white" {}		//baseTex
		_BaseColor("Base Color",Color) = (1,1,1,1)

		_NormalTex("Normal Texture",2D)="bump"{}
		
		_SpecPower("Specular Power",Float) = 1
		_SpecColor("Specular Color",Color) = (1,1,1,1)
		[KeywordEnum(ON,OFF)]_AllowShadow("Allow Shadow",Float)=1
	}
	SubShader
	{
		Tags 
		{ 
			"RenderType"="Opaque"
			"RenderPipeline" = "UnversalPipeline"
			"LightMode" = "UniversalForward"
			"Queue" = "Geometry"
		}

		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Lighting.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Shadows.hlsl"

		TEXTURE2D(_AlbedoTex);
		SAMPLER(sampler_AlbedoTex);
		TEXTURE2D(_NormalTex);
		SAMPLER(sampler_NormalTex);

		CBUFFER_START(UnityPerMaterial)
		float4 _BaseColor;
		float _SpecPower;
		float4 _SpecColor;
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

			#pragma shader_feature _ALLOWSHADOW_ON _ALLOWSHADOW_OFF

			//需要宏支持
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT


			v2f vert (a2v a)
			{
				v2f o;
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

				//获取阴影坐标系中的位置,并获取灯光参数
				float4 positionShadowCoord = TransformWorldToShadowCoord(i.positionWS);
				Light mainLightWithShadowAttenuation = GetMainLight(positionShadowCoord);

				float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv)).xyz;
				float3 normalDir = normalize(mul(tbn,normalTex));

				float NDL = dot(normalDir,normalize(mainLightWithShadowAttenuation.direction));
				float3 diffuseCol = SAMPLE_TEXTURE2D(_AlbedoTex,sampler_AlbedoTex,i.uv) * _BaseColor * (NDL * 0.5f + 0.5f) * mainLightWithShadowAttenuation.color;
				
				float3 viewDirWS =normalize(GetWorldSpaceViewDir(i.positionWS));
				float3 H = normalize(normalize(mainLightWithShadowAttenuation.direction) + viewDirWS);
				float NDH = saturate(dot(H,normalDir));
				float3 specColor = _SpecColor * pow(NDH,_SpecPower);

				//叠加灯光的衰减，如果上面GetMainLight不传入shadowsCoord shadowAttenuation = 1.0 ， 漫反射和高光都需要叠加灯光衰减
				float3 finalColor = (diffuseCol + specColor);

				#if _ALLOWSHADOW_ON
					finalColor *= mainLightWithShadowAttenuation.shadowAttenuation;
				#endif

				return float4(finalColor,1.0);
			}
			ENDHLSL
		}
		UsePass "Universal Render Pipeline/Lit/ShadowCaster"
	}
}
