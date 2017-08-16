/*
	Dirty Lens
	镜头光晕
*/
Shader "Overlay/LensFlare"
{
	Properties
	{
		_FlareTexture("Flare Texture",2D) = "white"{}
	}
	SubShader
	{
		Pass
		{
			Tags{ "Queue" = "Overlay" }
			ZWrite Off			//不需要写深度
			ZTest Always

			Blend One One 

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
				float3 worldPos:TEXCOORD1;
			};

			sampler2D _FlareTexture;
			float4 _FlareTexture_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _FlareTexture);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//摄像机的正方向
				float3 viewForward = UNITY_MATRIX_IT_MV[2].xyz;

				float3 viewDir = UnityWorldSpaceViewDir(i.worldPos.xyz);
				float flareIntensity = dot(viewDir, viewForward);

				float4 col = tex2D(_FlareTexture, i.uv);
				col.rgb *= flareIntensity;
				
				return col;
			}
			ENDCG
		}
	}
}
