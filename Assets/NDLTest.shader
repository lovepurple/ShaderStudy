Shader "Specular/NDLTest" {
    Properties 
    {
        _MainTex("Main Texture",2D)="white"{}
		_MainColor("Main Color",Color) = (1,1,1,1)
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
            float4 _FresnelReflectColor;

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

                float4 col = tex2D(_MainTex,i.uv) * _MainColor;

                col *= (1-LDotN) * (1-LDotN) -0.5;

                
                return col;
            }
            ENDCG
        }
    }
}
