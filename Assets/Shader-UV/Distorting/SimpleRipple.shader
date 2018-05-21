/*
    Ripple uv Shader ,贴图中心为圆心扩散
*/
Shader "UVOperate/SimpleRipple" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", Range(0.5,500.0)) = 3.0
        _Speed ("Speed", Range(-50,50.0)) = 1.0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off
        Pass{
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            half4 _Color;
            half _Scale;
            half _Speed;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            struct v2f
            {
                float4 pos:SV_POSITION;     
                float2 uv:TEXCOORD0; 
            };


            v2f vert( appdata_base i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.texcoord,_MainTex);

                return o;
            }
            

            float4 frag (v2f i):COLOR
            {
                half2 uv = (i.uv- 0.5) * _Scale;
                half r = sqrt (uv.x*uv.x + uv.y*uv.y);
                half z = sin (r+_Time.y*_Speed) / r;

                float3 col =  _Color.rgb * tex2D (_MainTex, i.uv+z).rgb;

                return float4(col,1.0);
            }
            ENDCG
        }
    }
}