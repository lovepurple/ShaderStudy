/*
	六边形马赛克效果 
*/
Shader "Unlit/Hexagon"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Lenght("Lenght",float) = 1.0
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Lenght;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				float TR = 0.8660225f;
				float uvX = i.uv.x;
				float uvY = i.uv.y;

				int wx = int(uvX / 1.5f / _Lenght);
				int wy = int(uvY / 1.5f / _Lenght);
			    float2 v1, v2, vn;  
			    if(wx/2 * 2 == wx) {  
			        if(wy/2 * 2 == wy) {  
			                v1 = float2(_Lenght*1.5f*wx, _Lenght*TR*wy);  
			                v2 = float2(_Lenght*1.5f*(wx+1), _Lenght*TR*(wy+1));  
			            } else {  
			                v1 = float2(_Lenght*1.5f*wx, _Lenght*TR*(wy+1));  
			                v2 = float2(_Lenght*1.5f*(wx+1), _Lenght*TR*wy);  
			            }  
			        } else {  
			        if(wy/2 * 2 == wy) {  
			            v1 = float2(_Lenght*1.5f*wx, _Lenght*TR*(wy+1));  
			            v2 = float2(_Lenght*1.5f*(wx+1), _Lenght*TR*wy);  
			            } else {  
			                v1 = float2(_Lenght*1.5f*wx, _Lenght*TR*wy);  
			                v2 = float2(_Lenght*1.5f*(wx+1), _Lenght*TR*(wy+1));  
			            }  
			    }  
			    float s1 = sqrt( pow(v1.x-uvX, 2) + pow(v1.y-uvY, 2) );  
			    float s2 = sqrt( pow(v2.x-uvX, 2) + pow(v2.y-uvY, 2) );  
			    if(s1 < s2)  
			        vn = v1;  
			    else  
			        vn = v2;  

			     float4 col = tex2D(_MainTex,vn);
			     return col;
			}
			ENDCG
		}
	}
}
