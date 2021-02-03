/**
特效Shader中常用的一种流光算法

float flow =  saturate(pow(1-abs(frac(i.positionWS.y * 0.2 - _Time.y * 0.2)-0.5),10));

Pow的技巧，控制范围（BlinnPhong 中的 高光范围   假Fresnel 边缘光的范围控制也是使用pow）
*/
Shader "URP/Streamer"
{
	Properties
	{
		[HDR]_EmissionColor("Emission Color",Color)=(1,1,1,1)
		_FlowSpeed("Streamer Flow Speed",Float) = 1.0
		_StreamerWidth("Streamer Width",Float) = 1.0		//流光的范围
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline"="UniversalRenderPipeline"
			"Queue"="Transparent"
			"RenderType"="Transparent"
		}

		HLSLINCLUDE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

		CBUFFER_START(UnityPerMaterial)
		half4 _EmissionColor;
		float _FlowSpeed;
		half _StreamerWidth;
		CBUFFER_END

		struct a2v
		{
			float4 positionOS:POSITION;
			float2 uv:TEXCOORD;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float3 positionWS:TEXCOORD1;
			float2 uv:TEXCOORD;
		};

		ENDHLSL

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			HLSLPROGRAM
			#pragma vertex VERT
			#pragma fragment FRAG

			v2f VERT(a2v v)
			{
				v2f o;
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				o.uv =v.uv;
				o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
				return o;
			}

			half4 FRAG(v2f i):SV_TARGET
			{
				//使用worldPosition.y 而不是用uv.v 可以保证严格的从下到上
				//中间亮 两边暗的渐变过渡
				// abs  减去 0.5 用于控制后面的过渡区
				// i.positionWS.y * 0.2   常量用于控制 同时出现的数量
				float flow = saturate(pow(1-abs(frac(i.positionWS.y * 0.2 - _Time.y * _FlowSpeed)-0.5),_StreamerWidth));

				return flow * _EmissionColor;
			}
			ENDHLSL
		}
	}
}
