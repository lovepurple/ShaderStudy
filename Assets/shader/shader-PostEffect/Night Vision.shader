/*
	夜视仪效果
*/
Shader "PostEffect/Night Vision"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		
		_VignetteTex("Vignette Tex",2D) = "white"{}				   //晕影贴图
		_NightVisionColor("Night Vision Color",Color) = (0,1,0,1)  //夜视颜色

	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float4 screenPos : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 uv:TEXCOORD1;
			};

			sampler2D _VignetteTex;
			float4 _VignetteTex_ST;

			sampler2D _MainTex;
			float4 _NightVisionColor;



			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(_MainTex, v.uv);

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				//Get the distance to the camera from the depth buffer for this point
				float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r);
			//Actual distance to the camera
			float fragZ = i.screenPos.z;

			//If the two are similar, then there is an object intersecting with our object
			float factor = 1 - step(0.1, abs(fragZ - sceneZ));

			float4 col = factor * _Color;
			col.a = 1;
			return col;
			}
			ENDCG
		}
	}
}