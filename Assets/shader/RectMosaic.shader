Shader "Unlit/RectMosaic"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MosaicSizeX("Mosaic Size X",float) = 8
		_MosaicSizeY("Mosaic Size Y",float) = 8
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _MosaicSizeX;
			float _MosaicSizeY;

			//XXX_TexelSize 是内置变量，(1/Width,1/Height,Width,Height) 贴图的四个值
			float4 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				//映射UV到图的大小
				float2 uv = (i.uv * _MainTex_TexelSize.zw);

				/*
					原理：
						XXX_TexelSize 表示贴图本身的信息， 比如 1024 * 1024 相当于(1/1024,1/1024,1024,1024)
						_MosaicSize 表示的是float2(_mosiaSize,_MosaicSize)的区域

						例：
						1.先将uv映射到 (0,1024) 区域
						2.用uv / 马赛克大小(8 * 8 ) 得到的值取整 再乘 相当于 ，原来是 96~104之间的像素点，映射回的坐标是 96 整个区间段的值都是这个值，！！！除法的作用
						3.再将过滤过的值 uv还原回0,1区间，所以就可以得到一段区域内的取的是同一个像素，形成了一个个方块，也就是马赛克效果
				*/
				float2 mosaicSize = float2(_MosaicSizeX, _MosaicSizeY);

				uv = floor(uv / mosaicSize) * mosaicSize;
				i.uv = uv * _MainTex_TexelSize.xy;

				return tex2D(_MainTex, i.uv);

			}
			ENDCG
		}
	}
}
