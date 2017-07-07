/*
	夜视仪效果
*/
Shader "PostEffect/Night Vision"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}

		_VignetteTex("Vignette Tex",2D) = "black"{}				   //晕影贴图
		_NoiseTex("Noise Tex",2D) = "white"{}
		_NightVisionColor("Night Vision Color",Color) = (0,1,0,1)  //夜视颜色
		_Brightness("Brightness",Range(0,1)) = 0.5			//亮度
		_Contrast("Contrast",float) = 1				//对比度
		_Saturation("Saturation",float) = 1			//饱和度
		_RandomValue("Randon Value",float) = 0.5
	}

		SubShader
		{
			Tags{ "RenderType" = "Opaque" }

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
				float4 _MainTex_ST;

				sampler2D _NoiseTex;
				float4 _NoiseTex_ST;

				float4 _NightVisionColor;

				float _Brightness;
				float _Contrast;

				float _RandomValue;


				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.screenPos = ComputeScreenPos(o.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					 float4 maskCol = tex2D(_VignetteTex,i.uv);
					 float4 mainCol = tex2D(_MainTex, i.uv);
					 float4 noiseCol = tex2D(_NoiseTex, i.uv + float2(_RandomValue, _RandomValue) * 100);

					 //亮度
					 //mainCol.r * 0.299 + mainCol.g * 0.587 + mainCol.b * 0.114;
					 float luminance = dot(mainCol.rgb, float3(0.299, 0.587, 0.114));
					 luminance += (_Brightness - 0.5f);

					 float4 col = float4(luminance, luminance, luminance, 1.0f);
					 col.rgb += _NightVisionColor*noiseCol.rgb;
					 col.rgb *= maskCol.rgb;
					 col.rgb = pow(col, _Contrast);

					 return col;
				}
				ENDCG
			}
		}
}