Shader "Shadow/DiffuseReceivePCFShadow"
{
	Properties
	{
		_Color("Main Color",Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Main Tex",2D) = "white"{}
		_ShadowColor("Shadow Color",Color) = (0,0,0,0.6)
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent"
			"Queue" = "Transparent" }
			LOD 100

			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

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
				float4 worldPos:TEXCOORD1;
				float3 worldNormal:TEXCOORD2;
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _ShadowColor;

			uniform sampler2D _ShadowMap;
			uniform float4x4 _LightSpaceProjectionUVMatrix;			//灯光坐标系下物体的UV,顶点世界-> 灯光view -> 灯光Projection -> [-1,1]->[0,1]
			uniform float2 _PixelSize;			//像素大小



			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}

			inline float GetDepthFromShadowmap(sampler2D shadowMap, float2 uv)
			{
				float shadowMapDepth = DecodeFloatRGBA(tex2D(shadowMap, uv));
				return shadowMapDepth;
			}

			//blur的精度决定影子的锯齿
			inline float GetShadowAttenuation(sampler2D shadowMap, float2 pixelSize, float2 posUVOnShadowmap)
			{
				float attenuation = 0;

				for (int i = -1; i <= 1; ++i)
				{
					for (int j = -1; j <= 1; ++j)
					{
						float2 uvOffset = posUVOnShadowmap + float2(i, j) * _PixelSize;
						attenuation += GetDepthFromShadowmap(shadowMap, uvOffset);
					}
				}

				return attenuation / 9;
			}



			float4 frag(v2f i) : SV_Target
			{
				float4 baseCol = tex2D(_MainTex, i.uv);

				float3 posInShadowMap = mul(_LightSpaceProjectionUVMatrix, i.worldPos).xyz;

				float bias = 0.005;

				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(i.worldPos);
				bias = max(0.05 * (1.0 - dot(i.worldNormal, worldSpaceLightDir)), 0.005);			//视线跟法线角度越大，偏移越多
				float depth = posInShadowMap.z + bias;

				//深度是对Shadowmap上周围3x3 采样 取平均
				float shadowmapDepthAfterPCF = GetShadowAttenuation(_ShadowMap, _PixelSize, posInShadowMap.xy);

				float f = step(depth, shadowmapDepthAfterPCF);

				return _Color*baseCol * (1 - f) + f *lerp(baseCol, _ShadowColor, _ShadowColor.a);

			}
			ENDCG
		}
		}
}
