/*
    2022-09-01 17:19:49
    UV旋转
    [ cos  sin]
    [-sin  cos]
*/
Shader "URP/UVRotate"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _RotateCenter ("UVRotate Center", Vector) = (0.5, 0.5, 0, 0)
        _RotateAngle ("UVRotate Angle", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Opaque" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float2 _RotateCenter;
            float _RotateAngle;

        CBUFFER_END

        struct a2v
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        ENDHLSL

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            
            v2f vert(a2v a)
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
                o.uv = TRANSFORM_TEX(a.uv, _MainTex);
                
                return o;
            }

            
            float4 frag(v2f v) : SV_Target
            {
                //2D旋转矩阵
                /*
                    [cos  sin]
                    [-sin cos]
                */
                float cosTheta = cos(_RotateAngle);
                float sinTheta = sin(_RotateAngle);

                float2x2 rotateUVMatrix = float2x2(cosTheta, sinTheta, -sinTheta, cosTheta);

                //UV原点
                float2 uv = v.uv - _RotateCenter.xy ;
                uv = mul(rotateUVMatrix, uv) + _RotateCenter.xy;
                
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return col;
            }

            ENDHLSL
        }
    }
}
