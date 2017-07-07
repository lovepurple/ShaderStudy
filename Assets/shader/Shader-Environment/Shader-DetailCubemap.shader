/*
	局部修正算法，提供相对直接reflect采样更精细的环境反射
*/
Shader "Environment/Shader-DetailCubemap"
{
	Properties
	{
		_EnvironmentMap("Environment（Cubemap）",CUBE) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 reflectViewDir:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float3 worldNormal:TEXCOORD2;
			};

			samplerCUBE _EnvironmentMap;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 worldNormal = normalize(i.worldNormal);
				float3 reflectViewDir = reflect(-viewDir, worldNormal);



			}
			ENDCG
		}
	}
}
