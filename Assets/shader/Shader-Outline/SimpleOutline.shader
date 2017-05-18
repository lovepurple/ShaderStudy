/*
	基于扩大顶点方式实现描边效果
*/
Shader "Outline/SimpleOutline"
{
	Properties
	{
		_MainColor("Main Color",Color) = (0.5,0.1,0.6,1.0)
		_MainTex("Main Texture(RGB)",2D) = "white"{}
		_OutlineFactor("Outline Factor",Range(0,0.1)) = 0.01
		_OutlineColor("Outline Color",Color) = (0.5,0.5,0.5,1.0)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

			ZTest Off


			Pass
			{
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				float _OutlineFactor;
				float4 _OutlineColor;

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal:NORMAL;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};


				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex.xyz + v.normal * _OutlineFactor);

					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					return _OutlineColor;
				}
				ENDCG
			}

			//使用Vertex&fragement 把主颜色显示出来
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				float4 _MainColor;
				sampler2D _MainTex;
				float4 _MainTex_ST;

				struct v2f
				{
					float4 vertex:SV_POSITION;
					float2 uv:TEXCOORD0;
				};

				v2f vert(appdata_full v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex).xy;
					return o;
				}

				float4 frag(v2f i) :COLOR
				{
					 float4 col = tex2D(_MainTex,i.uv);

					 return col * _MainColor;
				}


				ENDCG

			}
		}
}
