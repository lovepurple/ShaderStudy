// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
	带有遮挡效果的X-Ray
*/
Shader "Unlit/X-RayAdvance"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Color",Color) = (1,1,1,1)

		_XRayTex("X-Ray Texture",2D) = "white"{}
		_XRayColor("X-Ray Color",Color) = (0.435, 0.851, 1, 0.419)

	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" }
		LOD 100

		//先画遮挡部分
		Pass
		{
			ZWrite Off
		//	ZTest Greater
			ZTest Always
			Blend One OneMinusSrcColor
			Lighting Off
			Cull Off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _XRayTex;
			float4 _XRayTex_ST;
			float4 _XRayColor;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				//Model->view相当于直接投在屏幕上，画面脑补

				float3 viewNormal = mul(UNITY_MATRIX_IT_MV, v.normal);

				/*normal的ViewSpace的意义：
					1. capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
     				   capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
     				   其中x,y的分量表示法线在View坐标系下x,y轴上的分量的dot,也就是如果是最边缘的比如normal=(1.0,0,0) 则表示的是在x上的分量是最大也就是边
     				2. 由于dot的结果是[-1,1]，要映射到UV坐标必须转换到(0,1)，所以需要 viewNormal.xy * 0.5 +0.5 ，原来的0,0就变成了0.5,0.5，原来的-1 变成
     				   如果需要边缘光，发光贴图应该是一个圆型，如果需要正对摄像机方向更亮（x-ray效果），需要贴图的中心是亮的（法线正对摄像机，NDotV = 0）

				*/
				
				float2 matCapUV = viewNormal.xy*0.5 + 0.5;
				o.uv = matCapUV;

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = tex2D(_XRayTex, i.uv )*_XRayColor;

				return col ;
			}
			ENDCG
		}
		
		Pass
		{
			ZTest LEqual
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainColor;

			v2f_img vert(appdata_img i)
			{
				v2f_img o;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv = i.texcoord;

				return o;
			}

			fixed4 frag(v2f_img i):COLOR
			{
				fixed4 col = tex2D(_MainTex,i.uv) * _MainColor;

				return col;
			}
			ENDCG
		}
		
	}
}
