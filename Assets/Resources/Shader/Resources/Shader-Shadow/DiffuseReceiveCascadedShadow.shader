/*
	基于CSM的ShadowMap
*/
Shader "Shadow/DiffuseReceiveCascadedShadow"
{
	Properties
	{
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_MainTex("Base (RGB)", 2D) = "white" { }
		[HideInInspector]_ShadowMapArray("Shadowmap Array",2DArray) = ""{}
		_ShadowColor("Shadow Color",Color) = (0.3,0.3,0.3,0.8)
	}

		SubShader
		{
			Pass
			{
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.5
				#include "UnityCG.cginc"

				float4 _Color;
				sampler2D _MainTex;
				float4 _ShadowColor;

				UNITY_DECLARE_TEX2DARRAY(_ShadowMapArray);
				uniform float4x4 _WorldToShadowMapUVMatrix[1];
				uniform float4 _ShadowMapParams;

				struct appdata
				{
					float4 vertex:POSITION;
					float2 uv:TEXCOORD0;
					float3 normal:NORMAL;
					float4 color:COLOR;
				};

				struct v2f
				{
					float4 pos:SV_POSITION;
					float2 uv:TEXCOORD0;
					float3 worldNormal:TEXCOORD1;
					float4 worldPos:TEXCOORD2;
				};

				v2f vert(appdata i)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(i.vertex);
					o.uv = i.uv;
					o.worldNormal = UnityObjectToWorldNormal(i.normal);
					o.worldPos = mul(unity_ObjectToWorld, float4(i.vertex.xyz, 1.0));

					return o;
				}

				float4 frag(v2f i) :COLOR
				{
					float4 mainColor = tex2D(_MainTex,i.uv);

					//ViewPos里z的大小确定用哪个Shadowmap
					//!!!在乘矩阵的时候 如果不是四维一定要把第四个值补上
					float3 viewPos = mul(UNITY_MATRIX_V, float4(i.worldPos.xyz,1.0));
					float vertexDepth = -viewPos.z;

					int shadowmapIndex = floor(vertexDepth / ((_ShadowMapParams.y - _ShadowMapParams.x) / _ShadowMapParams.z));
					shadowmapIndex = 0;
					float4x4 lightSpaceUVMatrix = _WorldToShadowMapUVMatrix[shadowmapIndex];

					float4 posOnShadowmap = mul(lightSpaceUVMatrix, i.worldPos);

					float shadowmapDepth = DecodeFloatRGBA(UNITY_SAMPLE_TEX2DARRAY(_ShadowMapArray,float3(posOnShadowmap.x,posOnShadowmap.y,shadowmapIndex)));
					float posZOnLightSpace = posOnShadowmap.z;


					float bias = 0.005;

					//Shadow with bias
					float3 worldSpaceLightDir = UnityWorldSpaceLightDir(i.worldPos.xyz);
					bias = max(0.05 * (1.0 - dot(i.worldNormal, worldSpaceLightDir)), 0.005);			//视线跟法线角度越大，偏移越多
					float depth = posZOnLightSpace + bias;

					float f = step(depth, shadowmapDepth);

					float4 finalCol = _Color*mainColor * (1 - f) + f *lerp(mainColor, _ShadowColor, _ShadowColor.a);

					return finalCol;

				}


				ENDCG

			}
		}
			FallBack "Diffuse"
}
