// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SimpleWave"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_Speed("Speed",float) = 2.0
	}
		SubShader
		{
			Pass
			{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Speed;
			float4 _MainTex_ST;

			struct v2f
			{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
//				
				float time = _Time * _Speed;

				float waveValueA = cos(time);
				v.vertex.y+=waveValueA;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			float4 frag(v2f o):COLOR
			{
				return tex2D(_MainTex,o.uv);
			}

			ENDCG
}
		}
}

