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
				o.screenPos.y = 1-o.screenPos.y;
					
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;

				fixed depthValue= tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
				fixed linerDepthValue = Linear01Depth(depthValue);

				col.rgb = linerDepthValue;
				col.a = 1.0;


				return col;
			}
			ENDCG
		}
	}
}
