Shader "Enviornment/Shader-BaseCubemap"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_EnviornmentMap("Enviornment Map",CUBE) = "black"{}
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
				float3 normal:NORMAL;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal:TEXCOORD0;
				float3 viewDir:TEXCOORD1;
				float2 uv:TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			samplerCUBE _EnviornmentMap;


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, o.vertex).xyz);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float3 viewReflect = reflect(normalize(i.viewDir), normalize(i.normal));
				float4 enviornmentCol = texCUBE(_EnviornmentMap, viewReflect);


				return  enviornmentCol;
		}
		ENDCG
	}
	}
}
