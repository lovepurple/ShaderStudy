/*
	理解深度图，顶点深度的原理
*/
Shader "Depth/DepthCompare"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos:TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			uniform sampler2D _CameraDepthTexture;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos.xy = ComputeScreenPos(o.vertex);
				o.screenPos.z = -mul(UNITY_MATRIX_MV, o.vertex).z;		//一定要取成-的

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float sceneDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.screenPos)).r);
			    //sceneDepth = (sceneDepth - i.screenPos.z) / (_ProjectionParams.z - _ProjectionParams.y);
				sceneDepth -= i.screenPos.z;

				return float4(sceneDepth, sceneDepth, sceneDepth, 1);
			}

		ENDCG
	}
	}
}
