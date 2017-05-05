//在屏幕空间做溶解，溶解是由物体在屏幕上的坐标决定
Shader "Dissolve/DissolveInScreenPos"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DissolveTex("Dissolve Tex",2D) = "white"{}
		_DissolveEdgeTex("Dissolve Edge Texture",2D) = "white"{}
		_DissolvePercentage("Dissolve Percentage",Range(0,1)) = 0.2
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
					float4 vertexScreenPos:TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				sampler2D _DissolveTex;
				float4 _DissolveTex_ST;

				sampler2D _DissolveEdgeTex;
				float4 _DissolveEdgeTex_ST;

				float _DissolvePercentage;
				float _DissolveEdgeWidth;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.vertexScreenPos = ComputeScreenPos(o.vertex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed2 wcoord = i.vertexScreenPos.xy / i.vertexScreenPos.w;
					fixed dissolveChannel = tex2D(_DissolveTex, wcoord).r;
					fixed dissolveScope = dissolveChannel-_DissolvePercentage;
					
					clip(dissolveScope);

					fixed v = smoothstep(_DissolvePercentage,_DissolvePercentage + _DissolveEdgeWidth,dissolveChannel);
					fixed v2 = step(dissolveScope,_DissolveEdgeWidth);

					col = v2 * tex2D(_DissolveEdgeTex,float2(v,0)) + (1-v2) * col;


					return col;
				}
			ENDCG
		}
		}
}
