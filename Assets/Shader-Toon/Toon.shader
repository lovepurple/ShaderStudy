Shader "Unlit/Toon"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OutlineWidth("Outline Width",Range(0.0,0.1)) = 0.01
		_OutlineColor("Outline Color",Color) = (0,1,0,1)
		_ToonEffect("Toon Effect",Range(0.0,1)) = 0.5

			//色阶层数！ 这个解释直观
		_Steps("Step",Range(0,9)) = 3
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			//Pass
			//{
			//	Cull Front
			//	ZWrite Off

			//	CGPROGRAM

			//	#pragma vertex vert
			//	#pragma fragment frag
			//	#include "UnityCG.cginc"

			//	float _OutlineWidth;
			//	float4 _OutlineColor;

			//	struct appdata
			//	{
			//		float4 vertex:POSITION;
			//		float3 normal:NORMAL;
			//	};

			//	struct v2f
			//	{
			//		float4 pos:SV_POSITION;
			//	};

			//	v2f vert(appdata i)
			//	{
			//		v2f o;
			//		//直接把顶点向法线方向扩，输入的是模型本地坐标系，直接扩就正常
			//		i.vertex.xyz += i.normal * _OutlineWidth;

			//		o.pos = mul(UNITY_MATRIX_MVP, i.vertex);

			//		return o;
			//	}

			//	float4 frag(v2f i):COLOR
			//	{
			//		return _OutlineColor;
			//	}


			//	ENDCG
			//}

			Pass
			{
				Tags{"LightMode" = "Forwardbase"}
				Cull Back


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
					float3 viewDir:TEXCOORD1;
					float3 lightDir:TEXCOORD2;
					float3 normal:TEXCOORD3;
					float4 vertexInWorldSpace:TEXCOORD4;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				float4 _LightColor0;
				float _Steps;
				float _ToonEffect;
				//float4 _WorldSpaceLightPos0;


				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					o.normal = UnityObjectToWorldNormal(v.normal);

					float4 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);

					o.viewDir = UnityWorldSpaceViewDir(vertexWorldPos.xyz);
					o.lightDir = UnityWorldSpaceLightDir(vertexWorldPos.xyz);
					o.vertexInWorldSpace = vertexWorldPos;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);

					float3 normal = normalize(i.normal);
					float3 lightDir = normalize(i.lightDir);
					float3 viewDir = normalize(i.viewDir);

					float NDotL = max(0, dot(normal, lightDir));
					
					//加亮漫反射
					float diff = (NDotL + 1) / 2;


					float distanceToLight = length(i.vertexInWorldSpace- _WorldSpaceLightPos0);
					//衰减
					float attenuation = 1 / distanceToLight;
					float toon = floor(attenuation * attenuation * _Steps) / _Steps;
					diff = lerp(diff, toon, _ToonEffect);
					

					return col * diff;
				}
				ENDCG
			}
		}
}
