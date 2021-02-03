/**
特效Shader中常用的一种流光算法

float flow =  saturate(pow(1-abs(frac(i.positionWS.y * 0.2 - _Time.y * 0.2)-0.5),10));
*/
Shader "URP/Streamer"
{
	Properties
	{
		[HDR]_EmissionColor("Emission Color",Color)=(1,1,1,1)
		_StreamSpeed("Stream Speed",Float) = 1.0
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
		float _StreamSpeed;
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
				float flow = saturate(pow(1-abs(frac(i.positionWS.y * 0.2 - _Time.y * _StreamSpeed)-0.5),10));

				return flow * _EmissionColor;
			}
			ENDHLSL
		}
	}
}
