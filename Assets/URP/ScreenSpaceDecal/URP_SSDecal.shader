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
		_DecalTex ("Decal Texture", 2D) = "black" { }
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Transparent-499" }
		ZWrite Off
		ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

		struct a2v
		{
			float3 positionOS : POSITION;
			float2 uv0:TEXCOORD0;
		};

		struct v2f
		{
			float4 positionCS : SV_POSITION;
			float4 viewRayOS : TEXCOORD0;
			float4 positionSS : TEXCOORD1;
			float4 positionVS : TEXCOORD2;
			float4 positionWS : TEXCOORD3;
			float4 cameraPositionOS : TEXCOORD4;
			float2 uv:TEXCOORD5;
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

			v2f vert(a2v a)
			{
				v2f o;
				VertexPositionInputs vertexPosition = GetVertexPositionInputs(a.positionOS);
				o.positionCS = vertexPosition.positionCS;
				o.positionSS = ComputeScreenPos(o.positionCS);
				o.uv = a.uv0;

				//摄像机坐标系下的坐标
				//UNITY_MATRIX_IT_MV 是逆矩阵的转置，所以看上去出来的效果会有旋转
				//positionVS.xyz 是顶点在摄像机坐标系下的位置，w存
				float3 positionVS = vertexPosition.positionVS;
				
				float3 viewRayOS = mul(transpose((float3x3)UNITY_MATRIX_IT_MV), positionVS);

				//Unity ViewSpace的z值是反的

				o.viewRayOS.xyz = viewRayOS;
				
				o.positionVS.xyz = positionVS;

				o.positionWS.xyz = vertexPosition.positionWS;

				//摄像机位置转换到模型坐标系，以下两种方法，使用float4(0,0,0,1) 性能更好
				o.cameraPositionOS.xyz = TransformWorldToObject(float4(GetCameraPositionWS(), 1.0));
				// o.cameraPositionOS.xyz = mul(transpose(UNITY_MATRIX_IT_MV),float4(0,0,0,1)).xyz;
				return o;
			}

			float4 frag(v2f v) : SV_TARGET
			{
				//通过屏幕深度图获取当前像素在屏幕空间的深度
				float2 screenUV = v.positionSS.xy / v.positionSS.w;
				float screenDepth = tex2D(_CameraDepthTexture, screenUV);
				float sceneDepthRaw = LinearEyeDepth(screenDepth, _ZBufferParams);

				float3 cameraPositionOS = mul(UNITY_MATRIX_I_M, float4(GetCameraPositionWS(), 1.0));
				float3 positionVS = mul(UNITY_MATRIX_V, v.positionWS).xyz;
				float distance = positionVS.z;

				//顶点指向摄像机的射线
				float3 vertexToCameraRay = positionVS * - 1;
				float4x4 viewToObjectMatrix = transpose(UNITY_MATRIX_IT_MV);

				//处理的是方向 (3x3 不处理位移)
				float3 vertexToCameraRayDirOS = mul((float3x3)viewToObjectMatrix, vertexToCameraRay);

				//摄像机位置+ 方向 / 原长度分量 * 现有深度分量 = 贴花坐标
				float3 decalPosition = cameraPositionOS + vertexToCameraRayDirOS / distance * sceneDepthRaw;

				//通过ddx ddy 构建贴画坐标系
				float3 decalSpaceYAxis = normalize(cross(ddx(decalPosition), ddy(decalPosition)));

				//采样（使用Unity的Cube 是个单位1的）
				
				//位置超过0.5的都裁剪掉
				clip(0.5 - decalPosition);

				float2 decalUV = decalPosition.xy + 0.5;
				float4 decalTex = SAMPLE_TEXTURE2D(_DecalTex, sampler_DecalTex,decalUV);

				return float4(decalTex);




				
				

				// float3 positionVS = float3(v.positionVS.xy, v.positionVS.z);
				// cameraRay = normalize(v.positionVS);
				// float3 decalPostionVS = cameraRay * sceneDepthRaw;
				// float3x3 viewToObjectMatrix = transpose(UNITY_MATRIX_IT_MV);
				// float3 decalPositionOS = mul((float3x3)mul(UNITY_MATRIX_I_M,UNITY_MATRIX_I_V),positionVS);
				

				// return float4(v.cameraPostionOS2.xyz, 1);

			}

			ENDHLSL

		}
	}
	Fallback Off
}
