// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
	画两个圆
*/
Shader "Unlit/DoubleCircle"
{
	Properties
	{
		_CircleCenter("Circle Center",Vector) = (0.8,0.2,0.2,0.8)
		_Circle1Radius("Circle 1 Radius",float) = 100
		_Circle2Radius("Circle 2 Radius",float) = 100
		_Circle1Color("Circle 1 Color",Color) = (0.4,0.1,0.4,1.0)
		_Circle2Color("Circle 2 Color",Color) = (1.0,0.6,0.05)
		_BackgroundColor("Background Color",Color) = (0,0,0,1)
		_Antialias("Antialias Factor",float) = 10

		_LineWidth("Line Width",float) = 50
		_LineColor("Line Color",Color) = (0,0.7,0.8,1.0)
	}
		SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 _CircleCenter;
			float _Circle1Radius;
			float	_Circle2Radius;
			float4	_Circle1Color;
			float4 _Circle2Color;
			float4	_BackgroundColor;
			float	_Antialias;
			float _LineWidth;
			float4 _LineColor;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 vertexInScreenSpacePos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float4 vertexInScreenPos = ComputeScreenPos(o.vertex);
				float2 normalizeVertexInScreenPos = vertexInScreenPos.xy / vertexInScreenPos.w;
				o.vertexInScreenSpacePos = normalizeVertexInScreenPos;

				return o;
			}

			float4 drawCircleWithAntialias(float2 pixelPos,float2 circleCenter, float circleRadius, float antialias, float4 circleColor)
			{
				float antialiasEdge = length(pixelPos - circleCenter) - circleRadius;
				float smoothFactor = smoothstep(0, antialias, antialiasEdge);

				float4 circleCol = float4(circleColor.rgb, 1 - smoothFactor);
				return circleCol;
			}

			//画线：思路也是判断像素是否在线内
			//直线方程 ax + by +c = 0    y = k*x + b  dis = ax0 + by0 + c / sqrt(a*a + b *b)
			float4 drawLine(float2 pixel,float2 point1, float2 point2, float4 lineColor, float antialias, float lineWidth)
			{
				//求直线方程y = k*x + b(也可以用勾股定理结合dot求距离)   

				/*
					float2 lineDir = point1 - point2;  
					float2 pixelToPoint1 = pixel - point1;
					float LineDotPixelToPoint1 = dot(lineDir,pixelToPoint1)
					float disPoint1ToPixel = length(pixelToPoint1)
					float dis = sqrt(length( LineDotPixelToPoint1 * pixelToPoint1)^2 - disPointToPixel^2)
					
				*/


				float k = (point1.y - point2.y) / (point1.x - point2.x);
				float b = point1.y - k * point1.x;

				//点到直线的距离
				float pixelToLineDistance = abs(pixel.x * k + b - pixel.y) / sqrt(k*k + 1);

				//跟圆的类似
				float smoothFactor = smoothstep(0, antialias, pixelToLineDistance - lineWidth / 2.0);
				return float4(lineColor.rgb,1 - smoothFactor);
			}

			float4 frag(v2f i) : COLOR
			{
				float2 circle1Center = _CircleCenter.xy;
				float2 circle2Center = _CircleCenter.zw;

				float2 circle1CenterPixelCoord = circle1Center * _ScreenParams.xy;
				float2 circle2CenterPixelCoord = circle2Center * _ScreenParams.xy;

				float2 vertexPixelCoord = i.vertexInScreenSpacePos.xy * _ScreenParams.xy;

				float4 circle1Col = drawCircleWithAntialias(vertexPixelCoord,circle1CenterPixelCoord, _Circle1Radius, _Antialias, _Circle1Color);
				float4 circle2Col = drawCircleWithAntialias(vertexPixelCoord,circle2CenterPixelCoord, _Circle2Radius, _Antialias, _Circle2Color);


				//混色
				//两次混色，都是以上一次出来的背景做为下一次的混合因子
				float4 finalCol = lerp(_BackgroundColor, circle1Col, circle1Col.a);
				finalCol = lerp(finalCol, circle2Col, circle2Col.a);

				//画条线
				float4 lineCol = drawLine(vertexPixelCoord, circle1CenterPixelCoord, circle2CenterPixelCoord, _LineColor, _Antialias, _LineWidth);
				finalCol = lerp(finalCol, lineCol, lineCol.a);

				return finalCol;
			}
			ENDCG
		}
	}
}
