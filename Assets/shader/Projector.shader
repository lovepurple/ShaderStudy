/*
	Projector 组件使用的Shader
*/
Shader "Custom/Projector"
{
	Properties
	{
		_ShadowTex("Projected Image", 2D) = "white" {}
	}

		Subshader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
		//使用叠加的混合模式
		//Blend One One

		//透明混合
		Blend Zero OneMinusSrcAlpha

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform float4x4 _ObjectToProjectorMatrix;			//object space -> projector space

		sampler2D _ShadowTex;

		struct appdata
		{
			float4 vertex:POSITION;
			float2 normal:TEXCOORD0;
		};

		struct v2f
		{
			float4 pos:SV_POSITION;
			float4 posProj:TEXCOORD0;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.posProj = mul(_ObjectToProjectorMatrix, v.vertex);

			return o;
		}

		float4 frag(v2f i) : COLOR
		{
			if (i.posProj.w > 0.0)
			{
				float4 col = tex2D(_ShadowTex, i.posProj.xy / i.posProj.w);
				return col;
			}
			else
				return float4(0.0,0,0,0);
		}
	ENDCG
}
	}
}