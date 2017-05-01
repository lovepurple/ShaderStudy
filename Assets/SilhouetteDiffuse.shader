// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//基本的描边

Shader "Outlined/Diffuse" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (.5,.5,.5,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (.002, 0.03)) = .005
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }

    SubShader
    {
    	Tags {"LightMode"="Always"}
    	Cull Front
        ZWrite Off
        ColorMask RGB       //不使用Alpha
        ZTest Always

        Pass
        {
    	   CGPROGRAM

        	#pragma vertex vert
        	#pragma fragment frag
        	#include "UnityCG.cginc"

        	float4 _OutlineColor;
        	float _Outline;

        	struct appdata
        	{
        		float4 vertex:POSITION;
        		float3 normal:NORMAL;
        	};

        	struct v2f
        	{
        		float4 pos:SV_POSITION;
        	};

        	v2f vert(appdata v)
        	{
        		v2f o;
        		o.pos = UnityObjectToClipPos(v.vertex);

        		//ViewSpace里，z是向外，右手坐标系
        		//注意法线的变换跟正常顶点不一样
        		float3 normalInViewPos = mul(UNITY_MATRIX_IT_MV,v.normal);
        		float2 normalOffsetInProjection = TransformViewToProjection(normalInViewPos.xy);

        		//扩大顶点
        		o.pos.xy += normalOffsetInProjection * _Outline;

        		return o;
        	}

        	fixed4 frag(v2f i):COLOR
        	{
        		//return _OutlineColor;
        	   return float4(1,1,1,1);
            }

        	ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;

            struct appdata
            {
                float4 vertex:POSITION;
                float2 texcoord:TEXCOORD2;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos =UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            fixed4 frag(v2f i):COLOR
            {
                float4 tex = tex2D(_MainTex,i.uv) * _Color;
                return tex;
            }


            ENDCG
        }

    }

}