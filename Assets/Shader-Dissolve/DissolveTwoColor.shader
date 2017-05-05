Shader "Dissolve/DissolveTwoColor"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DissolveTex("Dissolve Tex",2D) = "white"{}
		_DissolvePercentage("Dissolve Percentage",Range(0,1)) = 0
		_DissolveEdgeInnerColor("Inner Edge Color",Color) = (1,0.5,0,1)
		_DissolveEdgeOuterColor("Outer Edge Color",Color) = (1,0,0)
		_DissolveEdgeWidth("Dissolve Edge Width",Range(0,0.1)) = 0.02
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

				sampler2D _DissolveTex;
				float4 _DissolveTex_ST;

				fixed _DissolvePercentage;

				float4 _DissolveEdgeInnerColor;
				float4 _DissolveEdgeOuterColor;

				fixed _DissolveEdgeWidth;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed dissolveChannel = tex2D(_DissolveTex, i.uv).r;
					fixed clipScope = dissolveChannel - _DissolvePercentage;

					clip(clipScope);

					/*
						1. _DissolvePercentage ~[0,1] 表示溶解的通道的范围
						2. 上方已经把r<= _DisslovePercentaget 都溶解掉了，所以边缘就是 [_DissolvePercentage ,_DissolvePercentage+_DissolveEdgeWidth] 这么宽
						3. 溶解的因子就是通道
					*/

					fixed v = smoothstep(_DissolvePercentage, _DissolvePercentage + _DissolveEdgeWidth, dissolveChannel);
					fixed v2 = step(clipScope, _DissolveEdgeWidth);

					col = v2 * lerp(_DissolveEdgeInnerColor, _DissolveEdgeOuterColor, v) + (1 - v2)*col;



					return col;
			}
			ENDCG
		}
		}
}
