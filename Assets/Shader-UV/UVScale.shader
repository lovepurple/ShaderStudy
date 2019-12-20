// UV缩放采样
Shader "UVOperate/UVScale"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        //uv 缩放比例
        _ScaleSize("UV Scale Size",Range(0.5,2.2))= 0.9

        //uv缩放中心
        _UVScalePivot("UV Pivot",Vector) = (0.5,0.5,0,0)        
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScaleSize;
            float4 _UVScalePivot;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // fixed2 scleraUV = i.uv.xy * _ScleraSize - (_ScleraSize - 1.0f) *_UVScalePivot;

                fixed2 scaledUV = (i.uv.xy -_UVScalePivot) * _ScaleSize + _UVScalePivot;
                fixed4 scleraTexColor = tex2D(_MainTex,scaledUV);

                return scleraTexColor;
              
            }
            ENDCG
        }
    }
}
