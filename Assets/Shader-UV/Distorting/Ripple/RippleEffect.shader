
Shader "Custom/RippleEffect" {
    Properties {
        //波纹变化梯度
        _RippleGradientTex("Ripple Gradient Tex",2D)="white"{}
        //折射强度(画面扭曲的强度)
        _RefractionIntensity("Refraction Intensity",Range(0,1)) = 0.2
        //反射颜色,波纹的颜色
        _ReflectionColor("Reflection Color", Color) = (0, 0, 0, 0)
        //反射强度
        _ReflectionIntensity("Reflection Intensity",Range(0,1)) = 0.2
        _RippleElapsedTime ("RippleElapsedTime", Float ) = 1
        _RippleCenterX ("RippleCenterX", Range(0, 1)) = 0.5
        _RippleCenterY ("RippleCenterY", Range(0, 1)) = 0.5
        _RippleSpeed("Ripple Speed",float) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma target 3.0

            uniform sampler2D _MainTex;

            fixed _RippleSpeed;
            float _RippleElapsedTime;
            float _RippleCenterX;
            float _RippleCenterY;
            fixed _RefractionIntensity;
            float4 _ReflectionColor;
            fixed _ReflectionIntensity;

            sampler2D _RippleGradientTex;

            //获取当前像素波纹的uv扭曲方向
            float GetRippleDirection(float2 currentUVCoord,float2 rippleCenterUVCoord)
            {
                float distanceToCenter = distance(currentUVCoord,rippleCenterUVCoord);
                float currentElapseTime = distanceToCenter / _RippleSpeed; 
                float deltaTime = _RippleElapsedTime - currentElapseTime;

                return (tex2D(_RippleGradientTex,float2(deltaTime,0)).a - 0.5f) * 2.0f;
            }

            float4 frag(v2f_img  i) : SV_Target
            {
                const float2 deltaX = float2(0.01f, 0);
                const float2 deltaY = float2(0, 0.01f);

                float2 aspectRate = float2(_ScreenParams.x / _ScreenParams.y,0);
                float2 uv = i.uv * aspectRate;

                float2 rippleCenterUVCoord = float2(_RippleCenterX,_RippleCenterY) * aspectRate;

                float currentRippleDirection = GetRippleDirection(uv,rippleCenterUVCoord);

                //？？？
                float deltaXDirection = GetRippleDirection(uv + deltaX,rippleCenterUVCoord) - currentRippleDirection;
                float deltaYDirection = GetRippleDirection(uv + deltaY,rippleCenterUVCoord) - currentRippleDirection;


                float2 deltaUV = float2(deltaXDirection,deltaYDirection) * 0.5f * _RefractionIntensity;

                float4 color = tex2D(_MainTex,i.uv + deltaUV);

                float reflectionFactor = pow(length(float2(deltaXDirection,deltaYDirection)) * 3 * _ReflectionIntensity,3);

                return lerp(color,_ReflectionColor,reflectionFactor);
            }
            ENDCG
        }
    }
}
