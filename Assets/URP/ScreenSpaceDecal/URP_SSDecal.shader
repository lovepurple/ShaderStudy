/**
URP下基本屏幕空间的贴画
1. 渲染深度图
2. 将此Shader应用在Cube上给上要帖到场景的物体周边
3. 通过摄像机坐标系建立贴画坐标系，z值使用深度图采样出的位置（紧贴场景）
**/
Shader "URP/URP_SSDecal" 
{
	Properties 
	{
		_DecalTex("Decal Texture",2D) = "white"{}
	}

	SubShader 
	{ 
		Tags {"RenderType"="Opaque" "Queue"= "Transparent-499" }
		ZWrite Off
		ZTest Off
		
		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

		struct a2v
		{
			float3 positionOS:POSITION;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float4 viewRayOS:TEXCOORD0;
			float4 positionSS:TEXCOORD1;
			float4 positionVS:TEXCOORD2;
			float4 positionWS:TEXCOORD3;
			float4 cameraPositionOS:TEXCOORD4;
			float4 cameraPostionOS2:TEXCOORD5;
		};

		TEXTURE2D(_DecalTex);
		SAMPLER(sampler_DecalTex);

		// TEXTURE2D(_CameraDepthTexture);
		// SAMPLER(sampler_CameraDepthTexture);
		sampler2D _CameraDepthTexture;

		// CBUFFER_START(UnityPerMaterial)
		// CBUFFER_END
		
		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert (a2v a) 
			{
				v2f o;
				VertexPositionInputs vertexPosition = GetVertexPositionInputs(a.positionOS);
				o.positionCS = vertexPosition.positionCS;
				o.positionSS = ComputeScreenPos(o.positionCS);

				//摄像机坐标系下的坐标
				//UNITY_MATRIX_IT_MV 是逆矩阵的转置，所以看上去出来的效果会有旋转
				float3 positionVS = vertexPosition.positionVS;
				float3 viewRayOS = mul(transpose((float3x3)UNITY_MATRIX_IT_MV),positionVS); 
				o.viewRayOS.xyz = viewRayOS;
			
				o.positionVS.xyz = positionVS;

				o.positionWS.xyz = vertexPosition.positionWS;

				o.cameraPositionOS.xyz = TransformWorldToObject( GetCameraPositionWS());
				o.cameraPostionOS2.xyz = mul(transpose((float3x3)UNITY_MATRIX_IT_MV),float4(0,0,0,1)).xyz;
				return o;
			}

			float4 frag(v2f v):SV_TARGET
			{
				float2 screenUV = v.positionSS.xy / v.positionSS.z;
				float screenDepth = tex2D(_CameraDepthTexture,screenUV);
				

				float3 positionVS = TransformWorldToView(v.positionWS.xyz);
				

				return float4(-positionVS.zzz,1);
			}

			ENDHLSL
		}
	}
	Fallback Off
}
