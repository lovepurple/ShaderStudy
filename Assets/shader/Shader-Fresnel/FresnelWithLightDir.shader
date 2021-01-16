/*
    菲涅尔反射 去除向光面
*/
Shader "Specular/FresnelWithLightDir" {
    Properties 
    {
        _MainTex("Main Texture",2D)="white"{}
		_MainColor("Main Color",Color) = (1,1,1,1)
        _FresnelPower("Fresnel Power",float) = 0.5
        _FresnelColor("Fresnel Reflect Color",Color)=(0,0.2,1,1.0)
        _FresnelRange("Fresnel Range",Range(0.01,5)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags 
            {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _FresnelPower;
            float4 _FresnelColor;
            float _FresnelRange;

			float4 _MainColor;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv:TEXCOORD0; 
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 viewDir:TEXCOORD2;
                float2 uv:TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                o.viewDir = UnityWorldSpaceViewDir(o.posWorld);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                i.viewDir = normalize(i.viewDir);

                float3 lightDir =normalize(UnityWorldSpaceLightDir(i.posWorld.xyz));
                float LDotN = max(0,dot(lightDir,i.normalDir));

                float4 col = _MainColor;

                //pow(1-LDotN,_FresnelRange) 用于控制光照面的Fresnel比例，如皮肤渲染中，向光面没有 Fresnel效果
                float4 fresnelCol =saturate(pow(1-LDotN,_FresnelRange))* pow(1 - max(0,dot(i.normalDir,i.viewDir)),_FresnelPower) * _FresnelColor;
                col.rgb += fresnelCol.rgb;
                
                return col;
            }
            ENDCG
        }
    }
}
