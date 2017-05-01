// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
	流光效果
*/

Shader "Unlit/X-RayAdvance2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor("Main Color",Color)=(1,1,1,1)

		_FlowTex("Flow Texture",2D)="white"{}
		_FlowColor("Flow Color",Color)=(1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
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
				float2 uvFlowLight:TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _FlowTex;
			float4 _FlowTex_ST;
			fixed4 _MainColor;
			fixed4 _FlowColor;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				float3 viewSpaceNormal = mul(UNITY_MATRIX_IT_MV,v.normal);
				o.uvFlowLight.xy = viewSpaceNormal.xy * 0.5 + 0.5;

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
				fixed4 flowCol = tex2D(_FlowTex,i.uvFlowLight) * _FlowColor;

				return col + flowCol ;
			}
			ENDCG
		}
	}
}
