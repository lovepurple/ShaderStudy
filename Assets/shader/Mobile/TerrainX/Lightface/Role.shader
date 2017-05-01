// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "GOE/Role"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RefTex ("Ref Tex (RGB)", 2D) = "black" {}
		_LightFactor ("Light Factor", Range(0,5)) = 0.2
		_ShadowSurfaceLight("ShadowSurfaceLight", Range(0,1)) = 0
	}
	
	Subshader
	{
		Tags { "Queue"="Geometry+200" "IgnoreProjector"="True" }
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			cull off
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers d3d11 flash
				#pragma multi_compile LIGHT_FACE_OFF LIGHT_FACE_ON 
				#include "UnityCG.cginc"
				#include "LightFace.cginc"
				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float4 uv 	: TEXCOORD0;
					float4 uv_ref_lightface	: TEXCOORD1;
					float4  depth : TEXCOORD2;
				};
				
				uniform float4 _MainTex_ST;
				float4x4 _texView;
				float _LightFactor;
				float _RoleLightAdjust;
				v2f vert (appdata_base v)
				{
					v2f o;
					float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex).xy;
					
					half2 capCoord;
					capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
					capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
					o.uv.zw = capCoord * 0.5f + 0.5f;
					
					float3 worldNormal = normalize(mul(unity_ObjectToWorld,  float4(v.normal.xyz, 0)));
					o.uv_ref_lightface.x = max(0.2,dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
					o.uv_ref_lightface.y = 0;
#ifdef LIGHT_FACE_ON	
					o.uv_ref_lightface.zw = LightFaceUV(worldPos).xy;
#else
					o.uv_ref_lightface.zw = half2(0,0);			
#endif
					o.depth = SimulateFogVS(o.pos.xyz, worldPos.xyz);
					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _RefTex;
				fixed4 _Color;
				fixed _ShadowSurfaceLight;
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv.xy);
					fixed4 ref = tex2D(_RefTex, i.uv.zw);
					
					tex.rgb = tex.rgb + tex.rgb * _LightColor0 * i.uv_ref_lightface.x  + (ref.rgb)*(tex.a*2);
#ifdef LIGHT_FACE_ON						
					LightFaceColor(tex.rgba, i.uv_ref_lightface.zw, fixed3(1,1,1), 0.15);
#endif	
					tex.rgb *= _Color.rgb;
					tex.xyz = SimulateFog(i.depth, tex, 1);

					tex.rgb *= (1-_ShadowSurfaceLight);
					tex.rgb *= (1 + _RoleLightAdjust);
					//fixed temp = (tex.x + tex.y +tex.z)* 0.35f;
					//tex.a = _LightFactor * temp;
					return tex;
				}
			ENDCG
		}
	}
	
}