Shader "Unlit/RimSword"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor("Main Color",Color)=(1,1,1,1)
		_EmissionAmount("Emission Amount",Range(0,50)) = 1
		_EmissionColor("Emission Color",Color) = (0,0,0,0)
		_EmissionPower("Emission Power",float) = 1

		_AllAlpha("All Alpha",Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+100" "IgnoreProjector"="True" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _AllAlpha;
			float _EmissionPower;
			float4 _EmissionColor;
			float _EmissionAmount;
			float4 _MainColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

	
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				// sample the texture
				float4 col = tex2D(_MainTex, i.uv) * _MainColor;
				


				return col;
			}
			ENDCG
		}
	}
}
