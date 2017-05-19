Shader "Outline/Outline-DepthTexture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Threshold("Depth Threshold",float) = 10
		_OutlineColor("Outline Color",Color) = (0,0,0,1)
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos:TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _CameraDepthTexture;
			float4 _OutlineColor;

			float _Threshold;
			

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.screenPos.z = -UnityObjectToViewPos(v.vertex).z;
				
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				float sceneDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r);
				float vertexDepth = i.screenPos.z;

				if (sceneDepth - vertexDepth > _Threshold)
					col = _OutlineColor;

			return col;
		}
		ENDCG
	}
	}
}
