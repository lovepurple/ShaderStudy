// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Heart"
{
	Properties
	{
		_BackgroundColor("Background Color",Color) = (1.0,0.8,0.7,1.0)
		_HeartColor("Heart Color",Color) = (1.0,0.5,0.3,1.0)

		//离心率
		_Eccentricity("Eccentricity",Range(0,0.5)) = 0.25
		_Blur("Edge Blur",Range(0,0.3)) = 0.01
		_Duration("Duration",Range(0.5,10.0)) = 1.5
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

			#pragma target 3.0  
			#include "UnityCG.cginc"
			#define PI 3.1415926


			float4 _BackgroundColor;
			float4 _HeartColor;
			fixed _Eccentricity;
			fixed _Blur;
			half _Duration;

			struct v2f
			{
				float4 srcPos:TEXCOORD0;
				float4 vertex : SV_POSITION;
			};


			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				//ComputeScreenPos Projection坐标系里的顶点在屏幕中的位置
				o.srcPos = ComputeScreenPos(o.vertex);
				return o;
			}

			float4 frag(v2f v) : COLOR
			{
				float2 normalizeVertexInScreenSpace = v.srcPos.xy / v.srcPos.w;
				float2 pixelCoord = normalizeVertexInScreenSpace * _ScreenParams.xy;			//顶点的像素坐标

				//像素到屏幕中心向量(像素的距离，所以在公式里要去除一个系数) min(_ScreenParams.x,_ScreenParams.y) 转换到[-1xxx,1.xxx] [-1,1 ]区间 (0,0)就是屏幕中心
				//！！！！坐标的转换
				float2 pixelToCenter =(pixelCoord - float2(0.5, 0.5)* _ScreenParams.xy) /(0.5* min(_ScreenParams.x,_ScreenParams.y));		
				//如果是Screen.y > Screen.x  则 x~[-1.xxx,1.xxx] y~[-1,1] 屏幕中心是(0,0) 
				//屏的上下边界到中心距离是1，左右大于1 ， 这里直接取1做为最大的圆半径

				//圆心向上移动1/4
				float2 center = float2(0.5,5 /8.0);
				float2 centerPixelCoord = center * _ScreenParams.xy;
				pixelToCenter = (pixelCoord - centerPixelCoord) / (0.5* min(_ScreenParams.x, _ScreenParams.y));			//0.5*的使用就是坐标转换，使用屏幕像素的一半，转换到1的坐标系


				//中心最亮 越远越暗效果
				float3 col = float3(1.0, 0.8, 0.7 - 0.07 * pixelToCenter.y) * (1- length(pixelToCenter)) *_BackgroundColor;

				//画心函数
				float a = atan2(pixelToCenter.x, pixelToCenter.y) / PI;
				float r = length(pixelToCenter);
				float h = abs(a);
				float d = (13.0*h - 22.0*h*h + 10.0*h*h*h) / (6.0 - 5.0*h);

				float s = 1.0 - 0.5*clamp(r / d, 0.0, 1.0);
				s = 0.75 + 0.75*pixelToCenter.x;
				s *= 1.0 - 0.25*r;
				s = 0.5 + 0.6*s;
				s *= 0.5 + 0.5*pow(1.0 - clamp(r / d, 0.0, 1.0), 0.1);
				float3 hcol = float3(1.0, 0.5*r, 0.3)*s;
				hcol = _HeartColor.xyz *s;

				float3 fin = lerp(col, hcol, smoothstep(-0.01, 0.01, d - r));
				fin = lerp(col, hcol, smoothstep(-_Blur, _Blur, d - r));

				return float4(fin.rgb, 1.0) ;

			}
		ENDCG
	}
	}
}
