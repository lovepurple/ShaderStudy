// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/RadiusEdge"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Radius("Radius",Range(0,0.5)) = 0.1
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Radius;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainCol = tex2D(_MainTex,i.uv);

				float2 leftTop = float2(_Radius,1-_Radius);
				float2 leftDown = float2(_Radius,_Radius);
				float2 rightTop = float2(1 -_Radius,1 -_Radius);
				float2 rightDown = float2(1-_Radius,_Radius);		//定义float2的时候必须用float2() 不能直接 float2 xx = (a,n)

				if(i.uv.x <= _Radius)
				{
					if(i.uv.y <= _Radius)
					{
						float dis = distance(i.uv,leftDown);
						if( dis > _Radius)
							discard;
					}
				
					if(i.uv.y >= 1 - _Radius)
					{
						float dis = distance(i.uv,leftTop);
						if(dis > _Radius)
							discard;
					}
				}

				if(i.uv.x >= 1 - _Radius)
				{
					if(i.uv.y < _Radius)
					{
						float dis = distance(i.uv,rightDown);
						
						if(dis > _Radius)
							discard;
						
					}

					if(i.uv.y > 1 - _Radius)
					{
						float dis = distance(i.uv,rightTop);
						if(dis > _Radius)
							discard;
					}

				}




				return mainCol;
				
			}
			ENDCG
		}
	}
}
