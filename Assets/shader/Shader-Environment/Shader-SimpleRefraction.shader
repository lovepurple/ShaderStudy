Shader "Environment/Shader-SimpleRefraction"
{
	Properties
	{
		_EnvironmentCube("Enviornment Cube",CUBE) = "_Skybox" { TexGen CubeReflect }
		_RefractionFactor("Refraction Factor",float) = 1
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
				float3 worldPos:TEXCOORD1;
				float3 worldNormal:TEXCOORD2;
			};

			samplerCUBE _EnvironmentCube;
			float _RefractionFactor;
			
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
				float3 worldViewDir = UnityWorldSpaceViewDir(i.worldPos);
				float3 refractDir = normalize(refract(worldViewDir, i.worldNormal,1.0f/ _RefractionFactor));

				float4 col = texCUBE(_EnvironmentCube, refractDir);
				
				return col;
			}
			ENDCG
		}
	}
}
