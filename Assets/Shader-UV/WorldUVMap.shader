// 根据世界坐标采uv,https://madewith.unity.com/en/stories/dissolving-the-world-part-1
// 物体动的时候 uv会变

Shader "UVOperate/WorldUVMap"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Tranparent" "Queue"="Transparent" }
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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//ComputeScreenPos 的输出参数是模型坐标系
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//求出屏幕的uv坐标
				fixed2 wCoord = i.screenPos.xy / i.screenPos.w;


				fixed4 col = tex2D(_MainTex, wCoord);
			col.rg = i.screenPos.xy;
			col.b = 0;

				return col;
			}
			ENDCG
		}
	}
}
