Shader "Unlit/DrawCircle"
{
	Properties
	{
		_CircleRadius("Circle Radius",float) = 100
		//_CircleCenter("Circle Center",Vector) = (100,100,100,1)
		_CircleColor("Circle Color",Color) = (0.8,0.5,0.8,1.0)
		_OutColor("Out Color",Color) = (0,0,0,1)
		_Antialias("Antialias Factor",float) = 0.1		//抗锯齿
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _CircleRadius;
			float4 _CircleCenter;
			float4 _OutColor;
			float4 _CircleColor;
			float _Antialias;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				//顶点屏幕坐标
				float4 vertexScreenPos = ComputeScreenPos(o.vertex);
				float2 normalizeVertexScreenPos = vertexScreenPos.xy / vertexScreenPos.w;
				o.uv = normalizeVertexScreenPos;

				return o;
			}

			//抗锯齿效果
			float4 drawCircleBySmoothstepWithAntialias(float2 pixelCoord,float2 centerPos, float radius, float antialias,float4 innerColor)
			{
				float d = length(pixelCoord - centerPos) - radius;
				/*
					!!!抗锯齿方法1
					 smoothstep(edge0,edge1,x)
					 x <= edge0   返回0
					 x >= edge1   返回1
					 edge0 < x < edge1 返回(0,1)的平滑系数
					
					 如果点在圆内，Alpha = 1 也就是用圆内的颜色
						 点在过渡区外(antialias 外)，直接Alpha为0
						 点在过渡区( 0 < d < antialias )，返回平滑的(0,1)的值
				 */
				float smoothFactor = smoothstep(0, antialias, d);

				return float4(innerColor.rgb, 1 - smoothFactor);
			}

			float4 frag(v2f i) : COLOR
			{
				//1.像素计算
				float2 vertexPixelCoord = i.uv * _ScreenParams.xy;

				//float2 vertexToCenter = vertexPixelCoord - _CircleCenter;

				//以屏幕中心为圆心
				float2 screenCenterPixel = float2(0.5, 0.5) * _ScreenParams.xy;
				/*float2 vertexToCenter = vertexPixelCoord - screenCenterPixel;

				if (length(vertexToCenter) > _CircleRadius)
				{
					return _OutColor;
				}
				else
				{
					return _CircleColor;
				}
*/
				float4 circleColor = drawCircleBySmoothstepWithAntialias(vertexPixelCoord, screenCenterPixel, _CircleRadius, _Antialias, _CircleColor);

				//lerp颜色混合
				// lerp(a,b,factor) = a * (1 - factor ) + b * factor
				// a = 0 时，也就是圆内 颜色 = _OutColor * 1 + circleColor * 0 
				// a = 1 ..
				// a = (0,1) 之前时，用lerp插值混合出具体颜色
				return lerp(_OutColor, circleColor, circleColor.a);
			}
			ENDCG
		}
	}
}
