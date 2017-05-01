// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TrangleMosaic"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Length("Length",float) = 10
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
				float _Length;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : COLOR 
				{
					float2 pixelUV = i.uv * _MainTex_TexelSize.zw;

					//六角形所在的矩形
					//六角形的连长为a,则所在矩形的为 sqrt(3) / 2 * 2 ,2 *a

					float TR = sqrt(3) / 2.0f;
					float width = 2 * TR * _Length;

					//int leftBottomX = floor(pixelUV.x / width);
					//int leftBottomY = floor(pixelUV.y / height);
					return float4(1, 1, 1, 1);
				}
				ENDCG
			}
		}
}
