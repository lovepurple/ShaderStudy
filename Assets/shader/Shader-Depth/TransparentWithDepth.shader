Shader "Depth/TransparentWithDepth"
{
    Properties
    {
        _Color("Tint Color" , Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }

        Pass
        {

            Tags { "LightMode"="ForwardBase" }

            ZWrite On

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _Color;

            struct v2f
            {
                float4 pos: SV_POSITION;
                float2 uv : TEXCOORD2;
                float3 worldNormal:TEXCOORD1;
                float4 worldPos:TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);

                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                //diffuse
                float3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos.xyz); 
                float NDotL = max(0,dot(worldLightDir,i.worldNormal));
                fixed3 albedo =  _Color.rgb;

                //环境光
                fixed3 ambient = _LightColor0.xyz * albedo;

                //Diffuse
                fixed3 diffuse = albedo * max(0,NDotL);


                return fixed4(ambient+diffuse,_Color.a);
            }
            ENDCG
        }

    }
    FallBack "Unlit/Transparent" 
}
