// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33878,y:32926,varname:node_3138,prsc:2|emission-6154-RGB;n:type:ShaderForge.SFN_TexCoord,id:1535,x:32828,y:32883,varname:node_1535,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:6154,x:33521,y:32855,ptovrint:False,ptlb:_MainTex,ptin:__MainTex,varname:node_6154,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6655-OUT;n:type:ShaderForge.SFN_Time,id:5233,x:32412,y:32436,varname:node_5233,prsc:2;n:type:ShaderForge.SFN_Sin,id:3987,x:32653,y:32511,varname:node_3987,prsc:2|IN-5233-T;n:type:ShaderForge.SFN_Add,id:627,x:33052,y:32728,varname:node_627,prsc:2|A-6065-OUT,B-1535-U;n:type:ShaderForge.SFN_Append,id:6655,x:33266,y:32855,varname:node_6655,prsc:2|A-627-OUT,B-1535-V;n:type:ShaderForge.SFN_Multiply,id:6065,x:32828,y:32638,varname:node_6065,prsc:2|A-3987-OUT,B-1219-OUT;n:type:ShaderForge.SFN_Slider,id:1219,x:32414,y:32820,ptovrint:False,ptlb:Scope,ptin:_Scope,cmnt:每次的幅度,varname:node_1219,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;proporder:6154-1219;pass:END;sub:END;*/

Shader "Shader Forge/DirectSinUVOnlyX" {
    Properties {
        __MainTex ("_MainTex", 2D) = "white" {}
        _Scope ("Scope", Range(-1, 1)) = 0
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D __MainTex; uniform float4 __MainTex_ST;
            uniform float _Scope;
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
                float4 node_5233 = _Time + _TimeEditor;
                float2 node_6655 = float2(((sin(node_5233.g)*_Scope)+i.uv0.r),i.uv0.g);
                float4 __MainTex_var = tex2D(__MainTex,TRANSFORM_TEX(node_6655, __MainTex));
                float3 emissive = __MainTex_var.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
