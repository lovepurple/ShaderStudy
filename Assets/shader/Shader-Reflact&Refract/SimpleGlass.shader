/*
	最基本的反射与折射
*/
Shader "Unlit/SimpleGlass"
{
	Properties
	{
		_MainColor("Main Color(RGB)",Color) = (1,1,1,1)
		_EnvironmentCube("Environment Cube",CUBE) = "white"{}
		_RefractFactor("Refract Factor",float) = 1.0
		_LerpFactor("LerpFactor",Range(0,1)) =0.5 
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
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
			};

			float4 _MainColor;
			samplerCUBE _EnvironmentCube;
			float _RefractFactor;
			float _LerpFactor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 worldNormal = normalize(i.worldNormal);
				float3 reflectDir = reflect(-worldViewDir, worldNormal);
				float3 refractDir = refract(worldViewDir, worldNormal, _RefractFactor);

				float4 reflectCol = texCUBE(_EnvironmentCube, reflectDir);
				float4 refractCol = texCUBE(_EnvironmentCube, refractDir);

				float4 finalCol = lerp(reflectCol ,refractCol, _LerpFactor) * _MainColor;

				return finalCol;
				
			}
			ENDCG
		}
	}
}
