Shader "Custom/Water" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
    _MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {}
    _Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
    _BumpMap ("Normalmap", 2D) = "bump" {}
}
 
SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    Cull Off
    LOD 300
   
 
CGPROGRAM
#pragma surface surf Lambert alpha vertex:vert
 
sampler2D _MainTex;
sampler2D _BumpMap;
samplerCUBE _Cube;
 
fixed4 _Color;
fixed4 _ReflectColor;
 
struct Input {
    float2 uv_MainTex;
    float2 uv_BumpMap;
    float3 worldRefl;
    INTERNAL_DATA
};
 
void vert (inout appdata_full v) {
    float phase = _Time * 20.0;
    float offset = (v.vertex.x + (v.vertex.z * 0.2)) * 0.5;
    v.vertex.y = sin(phase + offset) * 0.2;
}
 
void surf (Input IN, inout SurfaceOutput o) {
    fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
    fixed4 c = tex * _Color;
    o.Albedo = c.rgb;
   
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
   
    float3 worldRefl = WorldReflectionVector (IN, o.Normal);
    fixed4 reflcol = texCUBE (_Cube, worldRefl);
//  reflcol *= 1.0;
    o.Emission = reflcol.rgb * _ReflectColor.rgb;
    o.Alpha = reflcol.a * 0.9;
//  o.Alpha = 0.8;
}
ENDCG
}
 
FallBack "Reflective/VertexLit"
}