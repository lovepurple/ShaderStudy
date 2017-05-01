// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
    最基本的描边效果
    1. 根据法线扩大模型的顶点 o.pos = mul(UNITY_MATRIX_MVP,float4(v.vertex.xyz + v.normal) * factor,1.0))
    2. 再使用另一个Pass处理本身的颜色   
    3. 使用两个Pass
    4.  ZWrite Off
        ZTest Always        
        
*/
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
        
        //ZWrite ZTest是对于每个Pass生效的，而不是整个SubShader

        Pass
        {
            Name "MainColor"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex:POSITION;
                float2 uv:TEXCOORD0;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };

            v2f vert(appdata i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;

                return o;
            }

            float4 frag(v2f i):COLOR
            {
                float4 mainColor = tex2D(_MainTex,i.uv) * _Color;
                return float4(mainColor.rgb,1.0);
            }


            ENDCG

        }

        //主要处理描边的Pass
        Pass
        {
            Name "Outline"

            //本次Pass把正面剔除，否则Outline会挡住之前一个Pass
            Cull Front

            //关闭ZWrite， ZBuffer相当于无穷大，LEqual指的是，如果像素 <= ZBuffer 直接渲染
            //这里两个都使用默认值也没有问题
            ZWrite Off
            //ZTest Greater       //使用Greater渲染出来的就是背面
            ZTest LEqual


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _OutlineColor;
            fixed _Outline;

            struct appdata
            {
                float4 vertex:POSITION;
                float3 normal:NORMAL;       //使用法线处理边缘顶点偏移
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
            };

            v2f vert(appdata i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);

                //法线的摄像机坐标系
                float3 normalInViewSpace = mul((float3x3)UNITY_MATRIX_IT_MV,i.normal);

                //View -> 投影坐标系，x y 的位置
                float2 offset = TransformViewToProjection(normalInViewSpace.xy);

                //计算偏移（描边的大小）
                o.pos.xy += offset * o.pos.z * _Outline;

                return o;
            }

            float4 frag(v2f i):COLOR
            {
                return float4(_OutlineColor.rgb,1.0);

            }

            ENDCG      

        }

    }
}
