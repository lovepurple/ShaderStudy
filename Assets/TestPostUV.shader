Shader "Unlit/TestPostUV"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos:TEXCOORD1;
				float3 cameraRay:TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _CameraDepthTexture;
			float2 _ScreenParam;

			float4x4 _CameraProjectionMatrix_Inverse;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.screenPos = ComputeScreenPos(o.vertex);
				
				float4 cameraRayToFarPanel = float4(v.uv * 2.0 - 1.0, 1, 1);
				cameraRayToFarPanel = mul(_CameraProjectionMatrix_Inverse, cameraRayToFarPanel);
				cameraRayToFarPanel /= cameraRayToFarPanel.w;
				o.cameraRay = cameraRayToFarPanel.xyz;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv2 = 0;
				uv2.xy = i.uv * 2.0 - 1;

				float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;

				depth = Linear01Depth(depth);

				float4 color = 0;
				color.rgb = i.cameraRay * depth;
				color.a = 1;

				

				return color;
			}
			ENDCG
		}
	}
}
