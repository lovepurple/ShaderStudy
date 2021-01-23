Shader "URP/URP_RimNormal"
{
	Properties
	{
		_BaseTex("Base Texture",2D) = "white"{}
		_BaseColor("Base Color",Color) = (1,1,1,1)
		
		_NormalMap("Normal Tex",2D) ="bump"{}

		_RimPower("Rim Power",Float) = 0
		_RimColor("Rim Color",Color) = (0,0,0,1)
		_RimRange("Rim Range",Range(0,2)) = 0
	}

	SubShader
	{
		Tags
		{ 
			"RenderType" = "Opaque"  
			"RenderPipeline"="UniversalPipeline"
			"LightMode"="UniversalForward"
		}

		
		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		ENDHLSL
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			TEXTURE2D(_BaseTex);
			SAMPLER(sampler_BaseTex);
			
			TEXTURE2D(_NormalMap);
			SAMPLER(sampler_NormalMap);

			CBUFFER_START(UnityPerMaterial)
			float4 _BaseTex_ST;
			float4 _NormalMap_ST;
			float4 _BaseColor;
			float4 _RimColor;
			float _RimRange;
			float _RimPower;
			CBUFFER_END

			struct a2v
			{
				float4 positionOS:POSITION;
				float2 uv:TEXCOORD0;
				float3 normalOS:NORMAL;
				float4 tangentOS:TANGENT;
			};

			struct v2f
			{
				float4 positionCS:SV_POSITION;
				float3 normalWS:TEXCOORD0;
				float2 uv:TEXCOORD1;
				float3 tangentWS:TEXCOORD2;
				float3 positionWS:TEXCOORD3;
			};

			v2f vert(a2v i)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
				o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
				o.normalWS = TransformObjectToWorldNormal(i.normalOS);
				o.uv = TRANSFORM_TEX(i.uv,_BaseTex);
				o.tangentWS = TransformObjectToWorldDir(i.tangentOS.xyz) * i.tangentOS.w;

				return o;
			}

			float4 frag(v2f i):SV_Target
			{
				float3 bitangentWS = cross(i.normalWS,i.tangentWS);
				float3x3 tbn =float3x3(
				float3(i.tangentWS.x,bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,bitangentWS.z,i.normalWS.z)
				);
				Light mainLightInfo = GetMainLight();
				
				float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap,sampler_NormalMap,i.uv));
				float3 normalWS =normalize(mul(tbn,normalTex));

				float3 viewDirWS =normalize(GetWorldSpaceViewDir(i.positionWS.xyz));
				float NDotV = saturate(dot(viewDirWS,normalWS));

				float3 rimColor = _RimColor *pow((1 - NDotV)*_RimRange,_RimPower);

				half3 lightDirWS = normalize(mainLightInfo.direction);
				float NDL = saturate(dot(normalWS,lightDirWS));
				float3 baseColor = SAMPLE_TEXTURE2D(_BaseTex,sampler_BaseTex,i.uv);
				float3 diffuseColor = baseColor * _BaseColor * mainLightInfo.color * (NDL * 0.5f +0.5f);

				return float4(diffuseColor + rimColor,1.0);
			}


			ENDHLSL

		}
	}
}
