// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:14,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33729,y:32722,varname:node_3138,prsc:2|emission-9490-RGB;n:type:ShaderForge.SFN_TexCoord,id:1021,x:32049,y:32581,varname:node_1021,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:5026,x:31862,y:32908,varname:node_5026,prsc:2;n:type:ShaderForge.SFN_Slider,id:3552,x:31705,y:33113,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_3552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:50;n:type:ShaderForge.SFN_Multiply,id:7538,x:32250,y:32990,varname:node_7538,prsc:2|A-5026-T,B-3552-OUT;n:type:ShaderForge.SFN_Multiply,id:4810,x:32250,y:32770,varname:node_4810,prsc:2|A-1021-V,B-8691-OUT;n:type:ShaderForge.SFN_Add,id:312,x:32498,y:32885,varname:node_312,prsc:2|A-4810-OUT,B-7538-OUT;n:type:ShaderForge.SFN_Append,id:9735,x:33078,y:33046,varname:node_9735,prsc:2|A-32-OUT,B-2911-OUT;n:type:ShaderForge.SFN_Vector1,id:2911,x:32974,y:33256,varname:node_2911,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:3598,x:33012,y:32725,varname:node_3598,prsc:2|A-1021-UVOUT,B-9735-OUT;n:type:ShaderForge.SFN_Tex2d,id:9490,x:33352,y:32856,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9490,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3598-OUT;n:type:ShaderForge.SFN_Sin,id:2284,x:32678,y:32909,varname:node_2284,prsc:2|IN-312-OUT;n:type:ShaderForge.SFN_Multiply,id:32,x:32797,y:33105,varname:node_32,prsc:2|A-2284-OUT,B-831-OUT;n:type:ShaderForge.SFN_Slider,id:8691,x:31705,y:32805,ptovrint:False,ptlb:Tiling,ptin:_Tiling,cmnt:Tiling 正弦波的稀疏度,varname:node_8691,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:20;n:type:ShaderForge.SFN_Slider,id:831,x:32428,y:33258,ptovrint:False,ptlb:Scope,ptin:_Scope,cmnt:全局稀疏度控制另一个参数,varname:node_831,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:9490-3552-8691-831;pass:END;sub:END;*/

Shader "UVOperate/SinDistort" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Speed ("Speed", Range(0, 50)) = 0
        _Tiling ("Tiling", Range(1, 20)) = 1
        _Scope ("Scope", Range(0, 1)) = 0
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
            
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _Speed;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Tiling;
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
                float4 node_5026 = _Time + _TimeEditor;
                float2 node_3598 = (i.uv0+float2((sin(((i.uv0.g*_Tiling)+(node_5026.g*_Speed)))*_Scope),0.0));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_3598, _MainTex));
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
