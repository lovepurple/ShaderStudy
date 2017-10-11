Shader "Hidden/DepthNormalTexture"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile SHOW_DEPTHNORMAL_NORMAL_ON SHOW_DEPTHNORMAL_NORMAL_OFF
			#pragma multi_compile SHOW_DEPTHNORMAL_DEPTH_ON SHOW_DEPTHNORMAL_DEPTH_OFF
			#pragma multi_compile SHOW_DEPTH_ON SHOW_DEPTH_OFF

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			uniform sampler2D _CameraDepthTexture;
			uniform sampler2D _CameraDepthNormalsTexture;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 screenPos:TEXCOORD1;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.vertex);

				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0.0)
						o.uv.y = 1.0 - o.uv.y;
				#endif


				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float depthValue;
				float3 viewNormalValues;		//ViewNormal

				//float3 nn = tex2D(_CameraDepthNormalsTexture, i.uv).xyz * float3(3.5554, 3.5554, 0) + float3(-1.7777, -1.7777, 1.0);
				//float g = 2.0 / dot(nn.xyz, nn.xyz);
				//float3 normal = float3(g * nn.xy, g - 1.0); // View space normal
				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.screenPos), depthValue, viewNormalValues);

				float depthFromDepthTexture = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);

				float4 col = float4(1, 1, 1, 1);

				#if SHOW_DEPTHNORMAL_NORMAL_ON
					col.rgb = viewNormalValues;
					return col;
				#elif SHOW_DEPTHNORMAL_DEPTH_ON
					col.rgb = depthValue;
					return col;
				#elif SHOW_DEPTH_ON
					col.rgb = Linear01Depth(depthFromDepthTexture);
					return col;
				#endif


				return col;
			}
			ENDCG
		}
	}
}
