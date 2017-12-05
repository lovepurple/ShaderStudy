Shader "Hidden/SceneDepth"
{
	Properties
	{
	}
	SubShader
	{
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
				float2 depth : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = o.vertex.zw;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return EncodeFloatRGBA(i.depth.x / i.depth.y);			//不使用深度图，直接使用z并转换成颜色输出
			}
			ENDCG
		}
	}
}
