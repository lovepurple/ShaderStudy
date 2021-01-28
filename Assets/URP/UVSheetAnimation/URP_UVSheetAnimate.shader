Shader "URP/UVSheetAnimate" {
	Properties 
	{
		_SheetTex ("Sheet Texture", 2D) = "white" {}
		_SheetSize("Sheet Size",Vector)=(4,4,0,0)
		_FPS("FPS",Float) = 24
	}

	SubShader 
	{		
		Tags 
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
			"LightMode"="UniversalForward"
			"IgnoreProjector" = "True"
		}

		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"

		TEXTURE2D(_SheetTex);
		SAMPLER(sampler_SheetTex);

		CBUFFER_START(UnityPerMaterial)
		float4 _SheetTex_ST;
		float4 _SheetSize;
		float _FPS;
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

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert(a2v a)
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
				o.uv = TRANSFORM_TEX(a.uv,_SheetTex);

				return o;
			}

			float4 frag(v2f i):SV_TARGET
			{
				float2 sheetUV = (i.uv / _SheetSize.xy);
				int frameIndex = floor(frac(_Time.y * _FPS));

				int uvOffsetX =frameIndex- floor(frameIndex / _SheetSize.x) * _SheetSize.x;
				int uvOffsetY = float(frameIndex/ _SheetSize.x);
				
				float deltaU = (1 / _SheetSize.x) * floor(uvOffsetX);
				float deltaV = (1/ _SheetSize.y) * floor(uvOffsetY);

				// _Time.y / 1/FPS

				// sheetUV.x = i.uv.x / floor(_SheetSize.x) + frac((floor(_Time.y) / 25) / _SheetSize.x);
				// sheetUV.y = i.uv.y / floor(_SheetSize.y);

				float4 col = SAMPLE_TEXTURE2D(_SheetTex,sampler_SheetTex, sheetUV + float2(deltaU,deltaV));
				

				return col;

			}
			ENDHLSL
		}
	}
}
