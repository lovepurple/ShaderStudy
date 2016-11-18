Shader "Unlit/BaseDissolve"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SliceGuide("Slice Guide",2D)="white"{}
		_SliceAmount("Slice Amount",Range(0,1)) = 0.5

		//加入外发光
		_RimColor("RimColor",Color)=(1.0,1.0,1.0,1.0)
		_RimPower("RimPower",Range(0,10)) = 0.5
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
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				float2 uv2:TexCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SliceGuide;
			float _SliceAmount;
			float4 _RimColor;
			float _RimPower;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				float3 normalInWorldPos = UnityObjectToWorldNormal(v.normal);
				float4 vertexInWorldSpace = mul(unity_ObjectToWorld,v.vertex);
				float3 viewToVertexInWorldSpace = UnityWorldSpaceViewDir(vertexInWorldSpace.xyz);

				float NDotV = saturate(dot(normalize(viewToVertexInWorldSpace),normalInWorldPos));

				o.uv2.x = NDotV;

				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{

				float3 sliceColor = tex2D(_SliceGuide,i.uv).rgb - _SliceAmount;
				
				//clip函数，（任何一个通道）小于0的直接被干掉
				clip(sliceColor);

				float4 baseColor =  tex2D(_MainTex,i.uv);

				//dot(a,b)  a,b垂直时，dot = 0 重合时 = 1,
				//所以上方使用NDotV求出的值是边缘为0，正对是1，如果要边缘更亮，取反
				baseColor.rgb += pow( 1-i.uv2.x,_RimPower) * _RimColor;

				return baseColor;
			}
			ENDCG
		}
	}
}
