Shader "Dissolve/BaseDissolve"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DissolveTex("Dissolve Tex",2D) = "white"{}
		_DissolvePercentage("Dissolve Percentage",Range(0,1)) = 0
		_DissolveEdgeColor("Edge Color",Color)=(1,0.5,0,1)
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

			float4 _DissolveEdgeColor;

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

				if (clipScope < _DissolveEdgeWidth)
				{
					col = _DissolveEdgeColor;
				}
				
				

				return col;
		}
		ENDCG
	}
	}
}
