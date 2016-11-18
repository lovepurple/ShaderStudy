/*
	最基本的描边效果
	1. 根据法线扩大模型的顶点 o.pos = mul(UNITY_MATRIX_MVP,float4(v.vertex.xyz + v.normal) * factor,1.0))
	2. 再使用另一个Pass处理本身的颜色   
	3. 使用两个Pass，第二个Pass比第一个后渲染，也就是，对于这个例子，
		先处理顶点，计算
	4. 	ZWrite Off
		ZTest Always		
		

*/

Shader "Unlit/SimpleOutline"
{
	Properties
	{
		_OutlineFactor("Outline Factor",Range(0,0.1)) = 0.01
		_OutlineColor("Outline Color",Color) = (0.5,0.5,0.5,1.0)

		_MainColor("Main Color",Color) = (0.5,0.1,0.6,1.0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		ZWrite Off
		ZTest Always		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			fixed _OutlineFactor;
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

			
			v2f vert (appdata v)
			{
				v2f o;
				float4 targetVertex =float4( (v.vertex.xyz + v.normal * _OutlineFactor * 0.1),1.0);
				o.vertex = mul(UNITY_MATRIX_MVP,targetVertex);
				

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
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

			struct v2f
			{
				float4 vertex:SV_POSITION;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP,v.vertex);
				return o;
			}

			float4 frag(v2f i):COLOR
			{
				return _MainColor;
			}


			ENDCG

		}
	}
}
