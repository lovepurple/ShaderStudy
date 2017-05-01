Shader "Unlit/CircleMosaic"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MosaicRadius("Mosaic Radius",float) = 10
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
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _MainTex_TexelSize;
				float _MosaicRadius;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					//转换到贴图像素坐标
					float2 pixelUV = i.uv * _MainTex_TexelSize.zw;

					if (_MosaicRadius < 0)
						_MosaicRadius = 0;

					float diameter = 2.0f * _MosaicRadius;

					//该像素所属圆的正方形最底端坐标（画个图理解）
					float2 mosaicCenterUV = floor(pixelUV / diameter) * diameter + _MosaicRadius;

					//像素到马赛克中心的距离
					float pixelToMosaicCenterLenght = length(pixelUV - mosaicCenterUV);

					float2 finalUV = i.uv;
					if (pixelToMosaicCenterLenght < _MosaicRadius)
						finalUV = mosaicCenterUV * _MainTex_TexelSize.xy;

					return tex2D(_MainTex, finalUV);
				}
				ENDCG
			}
		}
}
