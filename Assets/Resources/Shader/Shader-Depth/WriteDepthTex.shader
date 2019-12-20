// 仅写入深度缓冲区，不输出任何颜色
// 用于在有遮挡时，半透明物体无法写入ZBuffer，用于占位

Shader "Depth/WriteDepthTex"
{
		Properties
		{
		}

		SubShader
		{
			//"LightMode"="ShadowCaster" 如果不加，则不会写入深度缓冲区
			//再有一种方式，使用Fallback "Diffuse" ，因为Unity默认的材质
			Tags { "RenderType" = "Opaque" "LightMode"="ShadowCaster"}	

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return fixed4(0,0,0,1);
			}
			ENDCG
		}
	}
}