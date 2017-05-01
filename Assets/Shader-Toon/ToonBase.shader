/*
	最基本简化颜色的卡通渲染Shader
*/
Shader "Unlit/ToonBase"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tooniness("Tooniness",Range(0.1,10)) = 5 
		_Ramp("Ramp Texture",2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

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
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float NDotL : TEXCOORD2;
				float NDotV : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Tooniness;
			sampler2D _Ramp;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float4 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);


				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 lightDir = UnityWorldSpaceLightDir(vertexWorldPos.xyz);
				float3 viewDir = UnityWorldSpaceViewDir(vertexWorldPos.xyz);

				o.NDotL = dot(worldNormal, lightDir);
				o.NDotV = dot(worldNormal, viewDir);


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col = floor(col * _Tooniness) / _Tooniness;


				float diffLight = max(0, i.NDotL);
				//diffLight = diffLight*0.5 + 0.5;

				float rimLight = max(0, i.NDotV);
				//rimLight = rimLight*0.5 + 0.5;

				fixed3 rampCol = tex2D(_Ramp, float2(diffLight, rimLight)).rgb;

				//col.rgb = col.rgb * rampCol;


				return col;
			}
			ENDCG
		}
	}
}
