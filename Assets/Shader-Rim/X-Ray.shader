Shader "Unlit/X-Ray"
{
	Properties
	{
		_RampTex("Ramp Texture",2D)="white"{}
		_Amount("Amount",Range(0,1)) = 0.1

	}
	SubShader
	{
		Tags { "Queue" = "Transparent" }
		LOD 100
		ZWrite Off
	
		Blend One Zero
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			
			#include "UnityCG.cginc"

			sampler2D _RampTex;
			float4 _RampTex_ST;
			float _Amount;


			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 uv2:TEXCOORD1;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float3 normalInWorldSpace = UnityObjectToWorldNormal(v.normal);
				float4 vertexInWorldSpace =mul(unity_ObjectToWorld,v.vertex);
				float3 viewToVertexDir = UnityWorldSpaceViewDir(vertexInWorldSpace.xyz);
				float NDotV = abs(dot(normalize(viewToVertexDir),normalInWorldSpace));
				o.uv2.x = NDotV;

				float3 viewNormal = mul(UNITY_MATRIX_IT_MV,v.normal);
				o.uv2.xy = viewNormal.xy*0.5 +0.5;
				//o.uv2.y = 0;

				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				// float4 rampCol =tex2D(_RampTex,i.uv2 * 1/_Amount);
				float4 rampCol =tex2D(_RampTex,i.uv2);
				rampCol.rgb *= 2;
				rampCol.a = 1;
				return rampCol;
			}
			ENDCG
		}
	}
}
