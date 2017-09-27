
Shader "Custom/PlanarShadowOnCustomPlanar" {
	Properties
	{
		_ShadowColor("Shadow Color",Color) = (0.5,0.5,0.5,0.7)
		_ShadowBias("Shadow Bias",Range(0.1,0.99)) = 0.3
	}
		SubShader
	{
		pass
		{
			Tags{ "LightMode" = "ForwardBase"  "Queue" = "Opaque" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On

			Stencil
			{
				Ref 0
				Comp Equal
				Pass IncrWrap
				ZFail Keep
			}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4x4 _World2Ground;
			uniform float4x4 _Ground2World;

			float4 _ShadowColor;
			half _ShadowBias;

			float4 vert(float4 vertex: POSITION) : SV_POSITION
			{
				float3 litDir;

				litDir = WorldSpaceLightDir(vertex);
				litDir = mul(_World2Ground,float4(litDir,0)).xyz;
				litDir = normalize(litDir);
				float4 vt;
				vt = mul(unity_ObjectToWorld, vertex);
				vt = mul(_World2Ground,vt);
				vt.x = vt.x - (vt.y / litDir.y)*litDir.x;
				vt.z = vt.z - (vt.y / litDir.y)*litDir.z;
				vt.y = _ShadowBias;
				vt = mul(_Ground2World,vt);
				vt = mul(unity_WorldToObject,vt);
				return UnityObjectToClipPos(vt);
			}

			float4 frag(void) : COLOR
			{
				return _ShadowColor;
			}
			ENDCG
	}
	}
}