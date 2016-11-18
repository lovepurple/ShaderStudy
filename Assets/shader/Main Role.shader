// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "GOE/Main Role"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RefTex ("Ref Tex (RGB)", 2D) = "black" {}
		_OcclusionTex("Occlusion Tex (RGB)", 2D) = "black" {}
	}
	
	Subshader
	{
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" }
		// 先画被遮挡部分
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			zwrite off
			ZTest Greater
			Blend Srcalpha oneminusSrcalpha
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			struct v2f
			{
				float4 pos	: SV_POSITION;
				float2 uv 	: TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				half2 capCoord;
				capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz, v.normal);
				capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz, v.normal);
				o.uv.xy = capCoord * 0.5f + 0.5f;
				return o;
			}
			uniform sampler2D _OcclusionTex;
			fixed4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_OcclusionTex, i.uv.xy);
				return tex;
			}
			ENDCG
		}
		
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
				v2f vert (appdata_base v)
				{
					v2f o;
					float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex).xy;
					
					half2 capCoord;
					capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
					capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
					o.uv.zw = capCoord * 0.5f + 0.5f;				//这里使用的是一种光照模型 Lit Sphere
					
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
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv.xy);
					fixed4 ref = tex2D(_RefTex, i.uv.zw);
					
					tex.rgb = tex.rgb + tex.rgb * _LightColor0 * i.uv_ref_lightface.x  + (ref.rgb)*(tex.a*2);
#ifdef LIGHT_FACE_ON						
					LightFaceColorRole(tex.rgba, i.uv_ref_lightface.zw, fixed3(1,1,1), 1);
#endif	
					tex.rgb *= _Color.rgb;
					tex.xyz = SimulateFog(i.depth, tex, 1 );
					
					return tex;
				}
			ENDCG
		}
	}
	
}
