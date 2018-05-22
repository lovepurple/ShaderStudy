// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:14,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34169,y:33222,varname:node_2865,prsc:2|emission-2388-RGB;n:type:ShaderForge.SFN_TexCoord,id:1600,x:32846,y:33059,varname:node_1600,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Sin,id:9525,x:33056,y:33119,varname:node_9525,prsc:2|IN-1600-V;n:type:ShaderForge.SFN_Tex2d,id:2388,x:33845,y:33186,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2388,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5033-OUT;n:type:ShaderForge.SFN_Append,id:4073,x:33464,y:33219,varname:node_4073,prsc:2|A-4809-OUT,B-7611-OUT;n:type:ShaderForge.SFN_Vector1,id:7611,x:33296,y:33315,varname:node_7611,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:5033,x:33637,y:33186,varname:node_5033,prsc:2|A-1600-UVOUT,B-4073-OUT;n:type:ShaderForge.SFN_Multiply,id:4809,x:33169,y:33281,varname:node_4809,prsc:2|A-9525-OUT,B-7336-OUT;n:type:ShaderForge.SFN_Slider,id:7336,x:32787,y:33418,ptovrint:False,ptlb:node_7336,ptin:_node_7336,varname:node_7336,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;proporder:2388-7336;pass:END;sub:END;*/

Shader "UVOperate/Sin/SimpleSinDistort" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _node_7336 ("node_7336", Range(0, 5)) = 0
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
            ZTest Always
            
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _node_7336;
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
                float2 node_5033 = (i.uv0+float2((sin(i.uv0.g)*_node_7336),0.0));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5033, _MainTex));
                float3 emissive = _MainTex_var.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
