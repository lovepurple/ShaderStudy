// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ToonOnePass"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color",Color) = (0,1,1,1)
		_RimPower("Rim Power",Range(0.1,8.0)) = 2.0
		_ToonEffect("Toon Effect",Range(0,1)) = 0.5		//分离颜色的程度
		_Steps("Steps of Toon",Range(0,9)) = 3
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



				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float3 normal:TEXCOORD1;
					float3 lightDir:TEXCOORD2;
					float3 viewDir:TEXCOORD3;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _OutlineColor;
				float _RimPower;
				float _ToonEffect;
				float _Steps;
				float4 _LightColor0;

				v2f vert(appdata_full v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.normal = UnityObjectToWorldNormal(v.normal);

					float4 vertexInWorldSpace = mul(unity_ObjectToWorld, v.vertex);
					o.lightDir = normalize(UnityWorldSpaceLightDir(vertexInWorldSpace));
					o.viewDir = normalize(UnityWorldSpaceViewDir(vertexInWorldSpace));

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float4 col = tex2D(_MainTex,i.uv);
					float NDotV = saturate(dot(i.normal, i.viewDir));
					float NDotL = max(0, dot(i.normal, i.lightDir));

					//Rim 边缘光
					float rim = 1 - saturate(NDotV);	//
					rim = pow(rim, _RimPower);

					//边缘扩大范围，rim = 0.5时 =1  过渡更硬，把原来虚的地方都干掉范围0或1
					rim = floor(rim * 2);

					//rim 为0时，用的贴图颜色，（RimPower用于控制上方的dot区域）
					//col = lerp(col, _OutlineColor, rim);



					//暗部提亮处理！
					float diff = (NDotL + 1) / 2;

					diff = smoothstep(0,1,diff);

					float4 diffColor = col*_LightColor0 * diff;

					return diffColor;

					//float toon = floor(diff * _Steps) / _Steps;
					//diff = lerp(diff, toon, _ToonEffect);
					//col *= _LightColor0* diff;

					col *= _LightColor0*diff;


					return col + rim * _OutlineColor;


			}
			ENDCG
		}
		}
}
