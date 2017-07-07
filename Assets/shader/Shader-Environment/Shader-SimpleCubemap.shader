/*
	最基本的Cubemap反射，立方体贴图反射
	适合于近的，对于远的会失真。
*/
Shader "Environment/Shader-SimpleCubemap"
{
	Properties
	{
		_EnvironmentCube("Environment Cube",CUBE) = "white"{}
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldViewDir:TEXCOORD1;
				float3 worldNormal:TEXCOORD2;
			};

			samplerCUBE _EnvironmentCube;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldViewDir = UnityWorldSpaceViewDir(worldPos);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//worldViewDir要取反，否则反射出的结果是反的
				float3 viewReflect = reflect(-normalize(i.worldViewDir),normalize(i.worldNormal));
				fixed4 reflectCol = texCUBE(_EnvironmentCube, viewReflect);
				reflectCol.a = 1;
				return reflectCol;

			}
			ENDCG
		}
	}
}
