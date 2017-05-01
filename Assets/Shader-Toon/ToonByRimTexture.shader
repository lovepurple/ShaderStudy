Shader "Unlit/ToonByRimTexture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color",Color) = (0,1,1,1)
		_MainColor("Main Color",Color) = (1,1,1,1)
		_RimTex("Rim Texture", 2D) = "white"{}
		_OutlineWidth("Outline Width",Range(0,0.1)) = 0.02
		_Factor("Factor",Range(0,1)) = 0.5
		_ToonEffect("Toon Effect",Range(0,1)) = 0.5
	}
		SubShader
		{
			//两次Pass，第一次还是扩边缘法线

			Pass
			{
				Tags{ "LightMode" = "Always" }
				Cull Front


				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};

				float4 _OutlineColor;
				float _OutlineWidth;
				float _Factor;


				v2f vert(appdata_tan v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					float3 normalInViewSpace = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal.xyz);
					float2 normalInProjection = TransformViewToProjection(normalInViewSpace);

					o.vertex.xy += normalInProjection * _OutlineWidth;


					return o;
					/*v2f o;
					float3 dir = normalize(v.vertex.xyz);
					float3 dir2 = v.normal;
					float D = dot(dir, dir2);
					dir = dir*sign(D);
					dir = dir*_Factor + dir2*(1 - _Factor);
					v.vertex.xyz += dir*_OutlineWidth;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					return o;*/
				}

				fixed4 frag(v2f i) : SV_Target
				{
				return _OutlineColor;
				}
			ENDCG
		}
		}
}
