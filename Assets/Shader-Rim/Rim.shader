// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

/*
	外发光效果
*/
Shader "Unlit/Rim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RimColor("Rim Color",Color) = (1.8,0.2,0.5,1.0)
		_RimPower("Rim Power",Range(0,10)) = 2
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

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 uv2:COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _RimColor;
			half _RimPower;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);		//TRANSFORM_TEX()之后_MainTex_ST生效
					
				//顶点的世界坐标
				float4 vertexWorldPosition = mul(unity_ObjectToWorld,v.vertex);
				//顶点的摄像机到顶点的向量
				float3 vertexViewDir = UnityWorldSpaceViewDir(vertexWorldPosition.xyz);
				float3 normalInWorldPosition = UnityObjectToWorldNormal(v.normal);

				o.uv2.x =  saturate(dot(normalInWorldPosition,normalize(vertexViewDir)));

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainColor = tex2D(_MainTex,i.uv);
			mainColor.rgb += _RimColor.rgb * pow((1 - i.uv2.x), _RimPower);

				return mainColor;
			}
			ENDCG
		}
	}
}
