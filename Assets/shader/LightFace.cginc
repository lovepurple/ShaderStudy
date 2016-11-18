// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef LIGHTFACE_INCLUDED
#define LIGHTFACE_INCLUDED
#include "UnityShaderVariables.cginc"
#include "UnityLightingCommon.cginc"


float4x4 _texViewProj;
sampler2D _LightFace;
float4x4 _shadowViewProj;

fixed4 _FogColor;
fixed _FogIntensity;
float _FogStart;
float _FogEnd;
float _FogHeight;
fixed _LightFaceScale;
fixed _RoleLightAdjust;
fixed4 _ClearColor;
float _GameTime;

#ifdef LIGHTMAP_ON
#define LIGHTMAP_COORDS(idx1)		float4 texcoord1 : TEXCOORD##idx1;
#define LIGHTMAP_VS					o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#define LIGHTMAP_PS					fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv.zw));
#else
#define LIGHTMAP_COORDS(idx1)		
#define LIGHTMAP_VS					o.uv.zw = 0;
#define LIGHTMAP_PS					fixed3 lm = fixed3(1,1,1);
#endif

#ifdef LIGHT_FACE_ON
#define LIGHTFACE_COORDS(idx1)		half4 uvLightFace : TEXCOORD##idx1;
#define LIGHTFACE_VS				o.uvLightFace.xy = LightFaceUV(worldPos); o.uvLightFace.zw = 0;
#define LIGHTFACE_VS_SHADOW			o.uvLightFace.zw = LightFaceShadowUV(worldPos);
#define LIGHTFACE_PS				LightFaceColor(tex, i.uvLightFace.xy, lm, 1);
#define LIGHTFACE_PS_ROLE			LightFaceColorRole(tex, i.uvLightFace.xy, 1, 1);
#define LIGHTFACE_PS_SHADOW			LightFaceColorReceiveShadow(tex, i.uvLightFace.xy, i.uvLightFace.zw, lm, 1);
#else
#define LIGHTFACE_COORDS(idx1)
#define LIGHTFACE_VS
#define LIGHTFACE_VS_SHADOW
#define LIGHTFACE_PS				LightFaceColor(tex, 0, lm, 1);
#define LIGHTFACE_PS_ROLE			LightFaceColorRole(tex, 0, 1, 1);
#define LIGHTFACE_PS_SHADOW			LightFaceColorReceiveShadow(tex, 0, 0, lm, 1);
#endif

#define FOG_COORDS(idx1) half4 fogDepth : TEXCOORD##idx1;
#define FOG_VS o.fogDepth = SimulateFogVS(o.pos.xyz, worldPos.xyz);
#define FOG_PS tex.xyz = SimulateFog(i.fogDepth, tex, 1);

#define WORLD_POS float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

inline float4 SimulateFogVS(float3 screenPos, float3 worldPos)
{
	float4 o;
	o.x = screenPos.z * (1 - clamp(worldPos.y / _FogHeight, 0, 1));
	o.y = 0;
	o.zw = worldPos.xz / 200;
	return o;
}

inline fixed3 SimulateFog(float4 depth, fixed4 col, half fogScale)
{
	float fogFactor = (_FogEnd - abs(depth.x*fogScale)) / (_FogEnd - _FogStart);
	fogFactor = (1 - fogFactor)*_FogIntensity;
	fogFactor = clamp(fogFactor, 0.0, 1.0);
	fixed3 afterFog = _FogColor.rgb * (0.2 + _GameTime * 0.8) * fogFactor + (1 - fogFactor) * col.rgb;
	return afterFog;
}

inline half4 LightFaceUV(float4 worldPos)
{
	float4 viewPos = mul(_texViewProj, float4(worldPos.xyz, 1));
	viewPos = viewPos / viewPos.w;
	viewPos.xy = (viewPos.xy + fixed2(1.0f, 1.0f))/2.0f;
	return viewPos;
}

inline half4 LightFaceShadowUV(float4 worldPos)
{
	float4 viewPos = mul(_shadowViewProj, float4(worldPos.xyz, 1));
	viewPos = viewPos / viewPos.w;
	viewPos.xy = (viewPos.xy + fixed2(1.0f, 1.0f))/2.0f;
	return viewPos;
}

inline void LightFaceAlpha(inout fixed4 baseColor, float lightFactor)
{
	float brightness = (0.2990*baseColor.r + 0.587*baseColor.g + 0.114*baseColor.b);
	baseColor.a = ((brightness)* lightFactor);
}

inline void LightFaceColor(inout fixed4 baseColor, fixed2 uvLight, fixed3 lightmap, float lightFactor)
{
#ifdef LIGHT_FACE_ON	
	fixed4 lightColor = tex2D(_LightFace, uvLight).rgba;
#else
	fixed4 lightColor = _ClearColor + 0.5;
#endif
	baseColor.rgb = baseColor.rgb * lightColor * (1 + _LightFaceScale) * lightmap;
}

inline void LightFaceColorRole(inout fixed4 baseColor, fixed2 uvLight, fixed3 lightmap, float lightFactor)
{
#ifdef LIGHT_FACE_ON	
	fixed4 lightColor = tex2D(_LightFace, uvLight).rgba;
#else
	fixed4 lightColor = _ClearColor + 0.5;
#endif				
	baseColor.rgb = baseColor.rgb * (1 + _RoleLightAdjust) * lightColor * (1 + _LightFaceScale) *lightmap;
}

inline void LightFaceShadow(inout fixed4 baseColor, fixed2 uvShadow)
{
	fixed4 shadowColor = tex2D(_LightFace, uvShadow).rgba;	
	baseColor.rgb = baseColor.rgb * shadowColor.a;
}

inline void LightFaceColorReceiveShadow(inout fixed4 baseColor, fixed2 uvLight, fixed2 uvShadow, fixed3 lightmap, float lightFactor)
{
	LightFaceColor(baseColor, uvLight, lightmap, lightFactor);
#ifdef LIGHT_FACE_ON
	LightFaceShadow(baseColor, uvShadow);
#endif
}

inline void LightFaceColorWithNormal(inout fixed4 baseColor, fixed2 uvLight, fixed3 lightmap, float lightFactor)
{
}

inline void LightFaceColorReceiveShadowWithNormal(inout fixed4 baseColor, fixed2 uvLight, fixed2 uvShadow, fixed3 lightmap, float lightFactor)
{
}

#endif