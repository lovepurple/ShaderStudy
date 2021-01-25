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
				float4 positionNDC:TEXCOORD4;
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
				o.positionNDC = GetVertexPositionInputs(i.positionOS).positionNDC;
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

				float3 positionNDC = i.positionCS.xyz / i.positionCS.w;

				//法线缩放算法  可以使用unpackNormalScale 这里自己计算 
				float3 tangentNormalDir = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv.xy)).xyz;
				tangentNormalDir.xy *= _NormalScale;
				tangentNormalDir.z = sqrt(1 - tangentNormalDir.x * tangentNormalDir.x - tangentNormalDir.y * tangentNormalDir.y);

				float3 normalDir = mul(tbn,tangentNormalDir);
				// normalDir.xy *= _NormalScale;
				// normalDir.z = sqrt(1 - normalDir.x * normalDir.x - normalDir.y * normalDir.y);			

				return float4(positionNDC.xy,0,1.0);
			}
			ENDHLSL
		}
	}
}