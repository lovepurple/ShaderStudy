// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ScreenPos"
{
	Properties
	{

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
#include"UnityCG.cginc"

			struct v2f
			{
				float4 pos:SV_POSITION;
				float4 posInScreenSpace:TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posInScreenSpace = ComputeScreenPos(o.pos);

				return o;
			}

			//画圆		vertexPos是顶点在屏幕坐标系的位置
			float4 drawCircle(float2 vertexPos, float2 center, float radius, float4 color)
			{	
				//像素点到圆心的距离(单位像素)
				float vertexToCenter = length(vertexPos - center);
				if (vertexToCenter <= radius)
					return float4(1, 1, 1, 1) * color;
				else
					return float4(0, 0, 0, 1);

			}

			fixed4 frag(v2f i) : COLOR
			{
				//!!!顶点坐标的转换，把顶点转换到屏幕的像素坐标
				//使用定点x y的值除以w的值，得出来的是屏幕 中归一化后的屏幕位置范围是[0,1],左下(0,0)右上(1,1)
				float2 vertexFragCoord = (i.posInScreenSpace.xy / i.posInScreenSpace.w);

				//屏幕像素坐标(800,600) 这种
				float2 pixelCoord = vertexFragCoord * _ScreenParams.xy;

				/*
					几个重要位置


					float2 pos = pixelCoord.xy / _ScreenParams.xy  pos.x~(0,1) pos.y~(0,1)
					float2 pos = pixelCoord.xy / min(_ScreenParams.x,_ScreenParams.y)   if resolution.x > resolution.y   pos.x~(0,1.xx) posy.y~(0,1)
					!!!!!乘减的作用！！！ 
					float2 pos = (pixelCoord.xy / _ScreenParams.xy)* 2 - 1   pos.x~(-1,1) pos.y~(-1,1)
					float2 pos = (2* pixelCoord.xy - _ScreenParams.xy) / min(ScreenParams.x,ScreenParams.y)    If iResolution.x > iResolution.y, pos.x ~ (-1.xx, 1.xx), pos.y ~ (-1, 1)  

				*/
				return float4(1, 1, 1, 1);


			}
			ENDCG
		}
	}
}
