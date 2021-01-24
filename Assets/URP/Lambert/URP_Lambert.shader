Shader "URP/URP_Lambert"
{
	Properties
	{
		_BaseMap("_BaseMap",2D)="white"{}
		_BaseColor("_BaseColor",Color) = (1,1,1,1)
		_NormalTex("_NormalTex",2D)="bump"{}
	}

	SubShader
	{
		Tags{ 
			"RenderType" = "Opaque"  
			"RenderPipeline"="UniversalPipeline"
			"LightMode"="UniversalForward"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			TEXTURE2D(_NormalTex);
			SAMPLER(sampler_NormalTex);
			
			CBUFFER_START(UnityPerMaterial)
			float4 _BaseColor;
			float4 _BaseMap_ST;
			CBUFFER_END
			struct a2v
			{
				float2 uv:TEXCOORD0;
				float4 positionOS:POSITION;
				float3 normalOS:NORMAL;
				float4 tangentOS:TANGENT;
			};

			struct v2f
			{
				float3 normalWS:TEXCOORD0;
				float3 tangentWS:TEXCOORD2;
				float2 uv:TEXCOOR1;
				float4 positionCS:SV_POSITION;
			};

			v2f vert (a2v v)
			{
				v2f output = (v2f)0;
				VertexPositionInputs vertexOutput = GetVertexPositionInputs(v.positionOS.xyz);
				output.positionCS = vertexOutput.positionCS;
				output.uv = TRANSFORM_TEX(v.uv,_BaseMap);
				output.normalWS = TransformObjectToWorldNormal(v.normalOS,true);
				output.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz,true) * v.tangentOS.w;
				
				return output;
			}

			float4 frag (v2f i) : SV_Target
			{	
				float3 bitangentWS = cross(i.tangentWS,i.normalWS);
				float3x3 tbnMatrix = float3x3(
					float3(i.tangentWS.x,bitangentWS.x,i.normalWS.x),
					float3(i.tangentWS.y,bitangentWS.y,i.normalWS.y),
					float3(i.tangentWS.z,bitangentWS.z,i.normalWS.z)
				);
				float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv));

				float3 normalDir = normalize(mul(tbnMatrix,normalTex));

				float4 texColor = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv);
				float3 color = texColor.rgb * _BaseColor.rgb;
				Light mainLightInfo = GetMainLight();
				float3 mainLightDirWS = normalize(mainLightInfo.direction).rgb;

				float NDL = dot(normalDir,mainLightDirWS);
				float3 outColor = color;
				
				outColor = outColor * mainLightInfo.color ;
				outColor *= (NDL * 0.5f +0.5f);			//Half Lambert
				
				return float4(outColor.rgb,1.0f);
				
			}
			ENDHLSL
		}
	}
}