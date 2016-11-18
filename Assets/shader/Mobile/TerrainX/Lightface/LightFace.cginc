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
	fogFactor = clamp(fogFactor, 0.0, 1.0);
	fogFactor = (1 - fogFactor)*_FogIntensity;
	fixed3 afterFog = _FogColor.rgb * fogFactor + (1 - fogFactor) * col.rgb;
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
	fixed4 lightColor = tex2D(_LightFace, uvLight).rgba;
	baseColor.rgb = baseColor.rgb * lightColor * (1 + _LightFaceScale) *lightmap;
}

inline void LightFaceShadow(inout fixed4 baseColor, fixed2 uvShadow)
{
	fixed4 shadowColor = tex2D(_LightFace, uvShadow).rgba;
	baseColor.rgb = baseColor.rgb * shadowColor.a;
}

inline void LightFaceColorReceiveShadow(inout fixed4 baseColor, fixed2 uvLight, fixed2 uvShadow, fixed3 lightmap, float lightFactor)
{
	LightFaceColor(baseColor, uvLight, lightmap, lightFactor);
	LightFaceShadow(baseColor, uvShadow);
}

inline void LightFaceColorWithNormal(inout fixed4 baseColor, fixed2 uvLight, fixed3 lightmap, float lightFactor)
{
}

inline void LightFaceColorReceiveShadowWithNormal(inout fixed4 baseColor, fixed2 uvLight, fixed2 uvShadow, fixed3 lightmap, float lightFactor)
{
}

#endif