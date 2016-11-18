Shader "Unlit/OutlineWithViewDirAndNormal"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color",Color)=(0,0,0,1)
		_OutlineWidth("Outline Width",float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
				float4 normal:NORMAL;
			};

			struct v2f
			{
				float4 pos:POSITION;
				float4 color:COLOR;
			};

			sampler2D _MainTex;
			float _OutlineWidth;
			float4 _OutlineColor;

			v2f vert (appdata v)
			{
				v2f o;
				
				// Shader传到Fragment里的都是Projection 坐标系
				//在计算光照的时候，只是使用系数，可根据习惯把中间的转换到世界坐标系便于理解	
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);

				float2 offset = mul(UNITY_MATRIX_MVP,v.normal).xy;

				o.pos.xy += offset * o.pos.z * _OutlineWidth;
				o.color = _OutlineColor;

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				return i.color;
			}
			ENDCG
		}
	}
}
