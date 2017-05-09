/*
	使用摄像机生成深度图
*/

Shader "Unlit/DepthTexture"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "UnityCG.cginc"

			uniform sampler2D _CameraDepthTexture;
			float _Distance;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 screenPos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
					
				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				float4 col;

				float depthValue= tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r; 
	
				float linerDepthValue = Linear01Depth(depthValue);
	
				col.rgb = linerDepthValue;
				col.a = 1.0;



				return col;
			}
			ENDCG
		}
	}
}
