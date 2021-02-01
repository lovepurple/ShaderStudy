
Shader "URP/URP_Billboard" {
	Properties 
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_ScaleX("Scale X",Float) = 5
		_ScaleY("Scale Y",Float) = 2
		//是否使用ViewSpace计算，
		//Billboard的两种方式 
		//1. 将vertex在ViewSpace中基算，
		//2. 将 Camera转换到Object中构建新的变换坐标系 ?????
		[KeywordEnum(ON,OFF)] _CalculateViaViewCoord("Calculate Via View Space",Float) = 1.0			
	}

	SubShader 
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
		}

		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
		
		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);

		CBUFFER_START(UnityPerMaterial)
		float _ScaleX;
		float _ScaleY;
		CBUFFER_END

		struct a2v
		{
			float4 positionOS:POSITION;
			float2 uv:TEXCOORD0;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float2 uv:TEXCOORD0;
		};

		ENDHLSL

		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _CALCULATEVIAVIEWCOORD_ON _CALCULATEVIAVIEWCOORD_OFF
			

			v2f vert(a2v a)
			{
				v2f o = (v2f)0;
				o.uv=a.uv;

				#ifdef _CALCULATEVIAVIEWCOORD_ON
					float4 centerPositionVS = mul(UNITY_MATRIX_MV,float4(0,0,0,1));
					//viewSpace中 z值是恒定的 也就是 (0,0,0)到摄像机的距离  centerPositonVS.z
					
					//在没有缩放的情况下，positionVS = centerPosition + xy   
					//如果有缩放需要外部传入   positionVS = centerPositonVS +  float4(a.positionOS.x * _ScaleX,a.positionOS.y *_ScaleY,0,0)
					float4 positionVS = centerPositionVS + float4(a.positionOS.x,a.positionOS.y,0,0) * float4(_ScaleX,_ScaleY,1,1);		//要用float4

					float3 positionOS = mul(UNITY_MATRIX_I_M, mul(UNITY_MATRIX_I_V,positionVS));
					positionOS.y = a.positionOS.y;
					o.positionCS = TransformObjectToHClip(positionOS);
					
				#else

					//使用构建新的模型坐标系的方式

					//摄像机在模型坐标系下的方向为 新的模型坐标系的Z轴   _WorldSpaceCameraPos - 0,0,0
					// Billboard 旋转的是x y 轴，z轴并不旋转
					// 理解 不出。。。
					float3 cameraPositonOS =TransformWorldToObject(_WorldSpaceCameraPos);
					float3 zAxis = normalize(cameraPositonOS - a.positionOS.xyz);
					float3 xAxis = normalize(cross(zAxis,float3(0,1,0)));
					float3 yAxis = normalize(cross(xAxis,zAxis));

					float3x3 billboardMatrix_T = {xAxis,yAxis,zAxis};
					float3 positionBillboard = mul(a.positionOS.xyz,billboardMatrix_T);
					o.positionCS = TransformObjectToHClip(positionBillboard);
				#endif

				return o;
			}

			float4 frag(v2f i) : SV_TARGET
			{
				float4 col = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
				
				return col;
			}
			ENDHLSL
		}
	} 
}
