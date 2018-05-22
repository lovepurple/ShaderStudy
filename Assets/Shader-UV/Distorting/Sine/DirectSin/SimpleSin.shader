/*
	简单的Sin uv动画
*/
Shader "UVOperate/SimpleSin"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Speed("Speed",Range(0,50))=0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			half _Speed;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv ;

				//为啥只加uv.x
				//float4 color = tex2D(_MainTex,uv +float2(sin(_Time.y *_Speed + uv.y * 5.0) * 0.05,0));
				float4 color = tex2D(_MainTex,uv + float2(sin(uv.y),0));
				return color;
				
			}
			ENDCG
		}
	}
}
