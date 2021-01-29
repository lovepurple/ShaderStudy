
Shader "URP/URP_Billboard" {
	Properties 
	{
		_MainTex("Main Texture", 2D) = "white" {}
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

			v2f vert(a2v a)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
				o.uv = a.uv;

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
