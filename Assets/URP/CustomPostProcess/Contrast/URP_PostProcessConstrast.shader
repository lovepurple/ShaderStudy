Shader "URP/URP_PostProcessConstrast" 
{
	Properties
	{
		[HideInInspector] _MainTex("Main Tex",2D)="white"{}
		_Constrast("Constrast Value",Range(0,1)) = 1
	}

	SubShader 
	{
		
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
		
		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"

		struct a2v
		{
			float4 positionOS:POSITION;
			float2 uv:TEXCOORD0;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float4 uv:TEXCOORD0;
		};

		TEXTURE(_MainTex);
		SAMPLER(sampler_MainTex)；

		CBUFFER_START(UnityPerMaterial)
		float _Constrast;
		CBUFFER_END

		ENDHLSL

		Pass 
		{
			//在脚本中可统一操作
			Name "Constrast"
			ZWrite Off
			Cull Off
			ZTest Always

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(a2v a)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
				o.uv = a.uv;

				#if UNITY_UV_STARTS_AT_TOP
					output.positionCS.y *= -1;
				#endif

				return o;
			}

			float4 frag(v2f i):SV_TARGET
			{
				float3 col = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv).xyz;
				col = lerp(float3(0.5,0.5,0.5),col,_Constrast);

				return float4(col,1.0);
			}

			ENDHLSL
		}
	} 
}
