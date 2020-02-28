Shader "Unlit/Rainbow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Range("Range",float)=1
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormalDir:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Range;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormalDir = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir =normalize(UnityWorldSpaceLightDir(i.worldPos.xyz));
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos.xyz));

                float NDotV = max(0,dot(i.worldNormalDir,viewDir));
                float fresnel = pow(1-NDotV,_Range);

                float4 col=(float4)1;
                half3 ClothGlossRainbow = (saturate(3*abs(1-2.0*frac(fresnel+half3(0,-1/3.0,1/3.0)))-1));
                col.rgb = ClothGlossRainbow;

                return col;
            }
            ENDCG
        }
    }
}
