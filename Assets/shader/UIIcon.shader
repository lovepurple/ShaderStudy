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

				//计算圆角，UV坐标转换取余和减的作用

				//将UV坐标原点移动到中心
				fixed2 uvCenter = i.uv - fixed2(0.5, 0.5);

				fixed radiusX = fmod(uvCenter.x, 0.4);			//建立四个角的坐标系(脑补)
				fixed radiusY = fmod(uvCenter.y, 0.4);
				fixed absoluteXArea = step(0.4, abs(uvCenter.x));			//在-0.4~0.4 之间的x y像素都是显示部分（alpha = 0）
				fixed absoluteYArea = step(0.4, abs(uvCenter.y));

				//用Alpha控制显示还是不显示


				/*
					在[-0.4,0.4] 区域，absoluteXArea = 0,absoluteYArea = 0  ，alpha = 1 - 0 = 1，所以内部0.4正方形区域alpha = 1
					在[0.4~0.5] 区域 ，判断radiusX 及radiusY 到四个角坐标系坐标原点(0,0)的值，超出半径0.1 部分的值为1， alpha = 1 - 1 * 1 * 1 = 0


				*/
				fixed alpha = 1 - absoluteXArea * absoluteYArea * step(0.1, length(float2(radiusX, radiusY)));			//裁剪掉的alpha为1 这步

				float hui = atan2(uvCenter.y, uvCenter.x);

				return fixed4(hui, hui, hui, alpha);

			}
			ENDCG
		}
		}
}
