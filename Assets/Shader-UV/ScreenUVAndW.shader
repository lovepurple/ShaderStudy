/*
	屏幕空间 w的理解
*/
Shader "BaseTheory/ScreenUVAndW"
{
	Properties
	{
		_MainTex("MainTex",2D) = "white"{}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque"  }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex; //Depth Text

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 screenPos:TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				/* 顶点屏幕uv的计算方法，如果使用uv还原顶点位置 ，可以反推
				o.screenPos = o.pos.xyw;			//clipspace w值是真正的z
				o.screenPos.y *= _ProjectionParams.x;		//x is 1.0 (or –1.0 if currently rendering with a flipped projection matrix), 是否是反的
				*/
				//三种方法一样
				o.screenPos = o.pos / o.pos.w;
				o.screenPos.y *= _ProjectionParams.x;

				//使用screenPos 在ps 里就不需要去转换原点了
				o.screenPos = ComputeScreenPos(o.pos);
				o.screenPos /= o.screenPos.w;



				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float2 screenUV = i.screenPos.xy;
				float4 finalColor = tex2D(_MainTex, screenUV);

				return finalColor;
			}

			ENDCG
		}
	}
		FallBack "VertexLit"
}