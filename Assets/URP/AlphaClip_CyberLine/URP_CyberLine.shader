/**
科技扫描线效果
*/
Shader "URP\URP_CyberLine"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_MainColor("Main Color",Color) = (1,1,1,1)
		_NormalTex("Normal Tex",2D)="bump"{}
		_NormalScale("Normal Scale",float) = 1
		_RimColor("Rim Color",Color) = (0.54,0.17,0.89,1.0)		
		_RimPower("Rim Power",Float) = 1

		_CyberTex("Cyber Line Texture",2D) = "white"{}
		_CyberScrollSpeed("Cyber line Speed",Float) = 1
	}
	SubShader
	{
		Tags
		{ 
			"RenderType" = "TransparentCutout"
			"Queue" = "AlphaTest"
			"RenderPipeline" = "UniversalPipeline"
			"LightMode" = "UniversalForward"
		}
		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"
		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert 
			#pragma fragment frag 

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			TEXTURE2D(_CyberTex);
			SAMPLER(sampler_CyberTex);

			TEXTURE2D(_NormalTex);
			SAMPLER(sampler_NormalTex);

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _CyberTex_ST;
			float4 _RimColor;
			float _RimPower;
			float4 _MainColor;
			float _CyberScrollSpeed;
			float _NormalScale;
			CBUFFER_END

			struct a2v
			{
				float4 positionOS:POSITION;
				float3 normalOS:NORMAL;
				float4 tangentOS:TANGENT;
				float2 uv:TEXCOORD;
			};

			struct v2f 
			{
				float4 positionCS:SV_POSITION;
				float3 normalWS:TEXCOORD0;
				float3 tangentWS:TEXCOORD3;
				float4 uv:TEXCOORD1;
				float3 positionWS:TEXCOORD2;
				float4 positionSS:TEXCOORD4;
			};

			v2f vert(a2v i)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
				o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
				o.uv.xy = TRANSFORM_TEX(i.uv,_MainTex);
				o.uv.zw = TRANSFORM_TEX(i.uv,_CyberTex);
				o.normalWS = TransformObjectToWorldNormal(i.normalOS);
				o.tangentWS = TransformObjectToWorldDir(i.tangentOS.xyz) * i.tangentOS.w;
				o.positionSS = ComputeScreenPos(o.positionCS) / o.positionCS.w;
				return o;
			}

			float4 frag(v2f i) : SV_TARGET 
			{
				float3 bitangentWS = cross(i.normalWS,i.tangentWS);
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,bitangentWS.z,i.normalWS.z)
				);

				//法线缩放算法  可以使用unpackNormalScale 这里自己计算 
				float3 tangentNormalDir = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv.xy)).xyz;
				tangentNormalDir.xy *= _NormalScale;
				tangentNormalDir.z = sqrt(1 - tangentNormalDir.x * tangentNormalDir.x - tangentNormalDir.y * tangentNormalDir.y);
				float3 normalDir = mul(tbn,tangentNormalDir);

				float3 viewDirWS =normalize(GetWorldSpaceViewDir(i.positionWS));
				float NDV = 1- saturate(dot(normalDir,viewDirWS));
				float3 rimColor = _RimColor * pow(NDV,_RimPower);

				float3 cyberlineColor = SAMPLE_TEXTURE2D(_CyberTex,sampler_CyberTex,i.positionSS +float2(0,-(_Time.y *_CyberScrollSpeed))).rgb;
				clip(cyberlineColor.r - 0.1f);

				float3 baseColor = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv.xy).rgb * _MainColor;

				return float4(baseColor + rimColor,1.0);
			}
			ENDHLSL
		}
	}
}