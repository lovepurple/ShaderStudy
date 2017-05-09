/*
	基于深度混合
*/
Shader "Depth/DepthBlend"
{

	Properties
	{
		_MainTex("Main Tex",2D)="white"{}
		_HightLightColor("Hightlight Color",Color) = (0.8,0.9,0,1)
		_HightLightDepth("Hightlight Depth",Range(0,1)) =0
	}

	SubShader
	{

		Tags{ "RenderType" = "Opaque" }

		Pass{

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"  

		sampler2D _MainTex;
		float4 _MainTex_ST;

		sampler2D _CameraDepthTexture;

		float4 _HightLightColor;
		float _HightLightDepth;

		struct appdata
		{
			float4 vertex:POSITION;
			float2 texcoord:TEXCOORD0; 
		};

		struct v2f
		{
			float4 vertex:SV_POSITION;
			float2 uv:TEXCOORD1;
			float4 vertexScreenPos:TEXCOORD0; 
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.vertexScreenPos = ComputeScreenPos(o.vertex);		//用投影坐标系的坐标

			return o;
		}

		float4 frag(v2f i):SV_Target
		{
			float4 col = tex2D(_MainTex,i.uv);

			//从深度图上获取像素的深度值
			float sceneZ = Linear01Depth(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.vertexScreenPos)).r);

			float diff = min ((abs(sceneZ - i.vertex.z)) / _HightLightDepth, 1);

			col = lerp(col,_HightLightColor,diff);

			col.a = 1;

			col.rgb = 1- sceneZ;

			return col;
		}
	
		ENDCG
	}
	}

}