/**
PBR中 Fresnel 使用的是Schlick近似
F = F0 + (1 - F0)(1 - cos(L,H))^ 5

LDH = VDH

F0 和两个介质有关
F0 =  ( n1 - n2 / n1+n2) ^2


https://github.com/miloyip/light2d/blob/master/fresnel.c

float schlick(float cosi, float cost, float etai, float etat) {
    float r0 = (etai - etat) / (etai + etat);
    r0 *= r0;
    float a = 1.0f - (etai < etat ? cosi : cost);
    float aa = a * a;
    return r0 + (1.0f - r0) * aa * aa * a;
}

F0 叫基础反射率，和材质有关，
一般金属的三个通道值不相等(金属有颜色)，并且  每个通道的值 > 0.04
常用的F0值

}
**/
Shader "PBR/PBR_Fresnel"
{
Properties
{
    _Gloss("Gloss",float) = 1
    _F0("Schlick-Fresnel F0",Color) =(0.04,0.04,0.04,1)
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
        #include "UnityStandardBRDF.cginc"

        float _Gloss;    
        float4 _F0;

        struct appdata
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float3 normalOS:NORMAL;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 positionCS : SV_POSITION;
            float4 positionWS:TEXCOORD1;
            float3 normalWS:TEXCOORD2;
        };


        v2f vert (appdata v)
        {
            v2f o;
            o.positionCS = UnityObjectToClipPos(v.positionOS);
            o.uv = v.uv;
            o.normalWS = UnityObjectToWorldNormal(v.normalOS);
            o.positionWS = mul(unity_ObjectToWorld,v.positionOS);
            return o;
        }

        float3 fresnelTerm(float3 f0,float LDH)
        {
            return f0 + (1-f0)* pow((1-LDH),5);
        }

        float4 frag (v2f i) : SV_Target
        {
            float3 viewDirWS = UnityWorldSpaceViewDir(i.positionWS.xyz);
            float3 lightDirWS = UnityWorldSpaceLightDir(i.positionWS.xyz);
            float3 halfDir = normalize(viewDirWS + lightDirWS);

            float NDH = max(0,dot(halfDir,lightDirWS));
            float spcular = pow(NDH,_Gloss);

            float LDH = saturate(dot(lightDirWS,halfDir));
            float HDV = max(0,dot(i.normalWS,viewDirWS));

            float rimTerm = pow((1-HDV) * 0.5,_Gloss);

            float3 fresnel = fresnelTerm(_F0.rgb,HDV);
            
            
            return float4(fresnel,1.0);
        }
        ENDCG
    }
}
}
