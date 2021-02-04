/**
	高光贴图的使用， 用贴图控制 BlinnPhong的范围
*/
Shader "URP/URP_SpecularMap" {
	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_SpecularTex("Specular Texture",2D)="white"{}
		_NormalTex("Normal Tex",2D)="bump"{}
		_SpecPower("Specular Power",Float) = 1
		_SpecColor("Specular Color",Color) = (0.8,0.8,0,1)
	}

	HLSLINCLUDE
	#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
	#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
	#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
	#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Lighting.hlsl"
	
	TEXTURE2D(_MainTex);
	SAMPLER(sampler_MainTex);

	TEXTURE2D(_SpecularTex);
	SAMPLER(sampler_SpecularTex);

	TEXTURE2D(_NormalTex);
	SAMPLER(sampler_NormalTex);

	CBUFFER_START(UnityPerMaterial)
	float4 _MainTex_ST;
	float4 _SpecularTex_ST;
	float4 _NormalTex_ST;
	float _SpecPower;
	float4 _SpecColor;
	CBUFFER_END
	
	struct a2v
	{
		float4 positionOS:POSITION;
		float3 normalOS:NORMAL;
		float4 uv:TEXCOORD0;
		float4 tangentOS:TANGENT;
	};

	struct v2f
	{
		float4 positionCS:SV_POSITION;
		float3 normalWS:TEXCOORD0;
		float3 tangentWS:TEXCOORD1;
		float3 bitangentWS:TEXCOORD2;
		float4 uv:TEXCOORD3;
		float3 positionWS:TEXCOORD4;
	};

	ENDHLSL

	SubShader 
	{	
		Tags
		{
			"RenderType" ="Opaque"
			"Queue" = "Geometry"
			"RenderPipeline" = "UniversalPipeline"
			"LightMode"="UniversalForward"
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert(a2v v)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				o.normalWS = TransformObjectToWorldNormal(v.normalOS);
				o.tangentWS = normalize(TransformObjectToWorldDir(v.tangentOS.xyz)) * v.tangentOS.w;
				o.bitangentWS = cross(o.normalWS,o.tangentWS);
				o.uv.xy = TRANSFORM_TEX(v.uv.xy,_MainTex);
				o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
				return o;
			}

			real4 frag(v2f i) : SV_TARGET
			{
				Light mainLightInfo = GetMainLight();
				float3 viewDirWS =normalize( GetWorldSpaceViewDir(i.positionWS));
				float3 lightDirWS = normalize(mainLightInfo.direction);
				float3 H = normalize(lightDirWS + viewDirWS);

				float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv.xy)).xyz;
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);
				float3 normalDirWS = normalize(mul(tbn,normalTex));

				float3 baseColTex = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv).xyz;
				float3 baseCol = baseColTex * (dot(normalDirWS,lightDirWS) * 0.5f +0.5f) * mainLightInfo.color;

				float3 specTex = SAMPLE_TEXTURE2D(_SpecularTex,sampler_SpecularTex,i.uv.xy);
				float3 specColor = _SpecColor * pow(saturate(dot(H,normalDirWS)),_SpecPower) * specTex;		//Lerp

				return real4(specColor + baseCol ,1.0);
			}

			ENDHLSL
		}
	}
}
