// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
	布料的反射算法叫“Fresnel 反射算法”，视线垂直平面，反射弱，视线与平面越小，返现越明显
	，当你站在水边观察水面时，水是透明的，反射很弱，但是当你离水面越远时，基本就看不到河面以下的部分了，反射很强
*/
Shader "Unlit/Cloth"
{
	Properties
	{
		_MainColor("Main Color",Color) = (1,1,1,1)
		_MainTex("Main Texture",2D) = "white" {}			//基本贴图
		_BumpMap("Normal Map",2D) = "bump"{}				//法线贴图
		_DetailBump("Detail Bump",2D) = "white"{}			//细节贴图

		//Fresnel反射相关参数
		_FresnelColor("Fresnel Color",Color) = (1,1,1,1)
		_FresnelPower("Fresnel Power",Range(0,12)) = 3

		_RimPower("Rim",Range(0,12)) = 3
		_SpecIntensity("Specular Intensity",Range(0,1)) = 0.2
		_SpecWidth("Specular Width",Range(0,1)) = 0.2
	}
		SubShader
		{
			//只有启用了LightMode之后，Lighting.cginc里的才生效，包括_LightColor0
			Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float3 texcoord0:TEXCOORD1;
					float3 texcoord1:TEXCOORD2;
					float3 texcoord2:TEXCOORD3;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _BumpMap;
				sampler2D _DetailBump;
				float4 _MainColor;
				float4 _FresnelColor;
				half _Fresnel;
				half _RimPower;
				half _SpecIntensity;
				half _SpecWidth;

				v2f vert(appdata_base v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					float3 lightDir = WorldSpaceLightDir(v.vertex);
					float3 viewDir = WorldSpaceViewDir(v.vertex);
					float3 worldNormal = UnityObjectToWorldNormal(v.normal);

					o.uv = v.texcoord;
					o.texcoord0 = lightDir;
					o.texcoord1 = viewDir;
					o.texcoord2 = worldNormal;

					return o;
				}

				float4 frag(v2f i) : COLOR
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);

					fixed3 baseNormal = UnpackNormal(tex2D(_BumpMap, i.uv));
					fixed3 detailNormal = UnpackNormal(tex2D(_DetailBump, i.uv));

					half3 halfVector = normalize(i.texcoord0 + i.texcoord1);
					half NDotL = saturate(dot(i.texcoord2, i.texcoord0));
					half NDotV = saturate(dot(i.texcoord2, i.texcoord1));
					half NDotH = saturate(dot(halfVector, i.texcoord2));

					//高光模型使用BiPhong NDotH
					fixed4 specCol = pow(NDotH, _SpecIntensity);


					fixed4 diffuseCol = col * NDotL * _LightColor0 * 1.5 + specCol;

					return float4(diffuseCol);
				}
				ENDCG
			}
		}
}
