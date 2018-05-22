// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9087,x:33701,y:33072,varname:node_9087,prsc:2|emission-5990-RGB;n:type:ShaderForge.SFN_TexCoord,id:8401,x:32586,y:33104,varname:node_8401,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:9702,x:32337,y:33317,varname:node_9702,prsc:2;n:type:ShaderForge.SFN_Slider,id:3100,x:32402,y:33518,ptovrint:False,ptlb:Scope,ptin:_Scope,varname:node_3100,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:6763,x:32777,y:33409,varname:node_6763,prsc:2|A-5136-OUT,B-3100-OUT;n:type:ShaderForge.SFN_Sin,id:5136,x:32535,y:33339,varname:node_5136,prsc:2|IN-9702-T;n:type:ShaderForge.SFN_Add,id:4411,x:32956,y:33171,varname:node_4411,prsc:2|A-8401-UVOUT,B-6763-OUT;n:type:ShaderForge.SFN_Tex2d,id:5990,x:33160,y:33171,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_5990,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4411-OUT;proporder:3100-5990;pass:END;sub:END;*/

Shader "UVOperate/Sin/DirectSinUVXY" {
    Properties {
        _Scope ("Scope", Range(-1, 1)) = 0
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _Scope;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_9702 = _Time + _TimeEditor;
                float2 node_4411 = (i.uv0+(sin(node_9702.g)*_Scope));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4411, _MainTex));
                float3 emissive = _MainTex_var.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
