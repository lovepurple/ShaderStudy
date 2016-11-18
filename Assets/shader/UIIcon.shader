Shader "Custom/UIIcon"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Radius("Radius",Range(0.5,0.7)) = 0.6
	}
		SubShader
		{
			Tags{"Queue" = "Transparent"}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Radius;

				fixed4 frag(v2f i) : COLOR
				{
					fixed4 col = tex2D(_MainTex, i.uv);

				float2 vertexToCenter = i.uv - float2(0.5, 0.5);
				float lenVertexToCenter = length(vertexToCenter);
				float alpha = step(lenVertexToCenter, _Radius);			//step函数 step(a,b)  b<a = 0 否则 1 smoothstep





					return fixed4(col.rgb, alpha);

				}
				ENDCG
			}
		}
}
