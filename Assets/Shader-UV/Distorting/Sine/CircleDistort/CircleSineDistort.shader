// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34354,y:33214,varname:node_3138,prsc:2|emission-9006-RGB;n:type:ShaderForge.SFN_Tex2d,id:9006,x:33947,y:33230,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9006,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5665-OUT;n:type:ShaderForge.SFN_Slider,id:4778,x:31577,y:32828,ptovrint:False,ptlb:CenterX,ptin:_CenterX,varname:node_4778,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:6746,x:31577,y:32972,ptovrint:False,ptlb:CenterY,ptin:_CenterY,varname:node_6746,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Append,id:5725,x:31939,y:32890,varname:node_5725,prsc:2|A-4778-OUT,B-6746-OUT;n:type:ShaderForge.SFN_TexCoord,id:2454,x:31939,y:32670,varname:node_2454,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Distance,id:46,x:32175,y:32816,varname:node_46,prsc:2|A-2454-UVOUT,B-5725-OUT;n:type:ShaderForge.SFN_Slider,id:2648,x:32003,y:33114,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_2648,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:120.3846,max:500;n:type:ShaderForge.SFN_Multiply,id:5290,x:32354,y:32970,varname:node_5290,prsc:2|A-46-OUT,B-2648-OUT;n:type:ShaderForge.SFN_Slider,id:6262,x:31999,y:33298,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_6262,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-50,cur:0,max:50;n:type:ShaderForge.SFN_Time,id:2166,x:32156,y:33406,varname:node_2166,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2559,x:32353,y:33352,varname:node_2559,prsc:2|A-6262-OUT,B-2166-T;n:type:ShaderForge.SFN_Add,id:4981,x:32571,y:33166,varname:node_4981,prsc:2|A-5290-OUT,B-2559-OUT;n:type:ShaderForge.SFN_Sin,id:7071,x:32746,y:33166,varname:node_7071,prsc:2|IN-4981-OUT;n:type:ShaderForge.SFN_Append,id:8644,x:33458,y:33134,cmnt:UV都叠加也可以只对某维度叠加,varname:node_8644,prsc:2|A-520-OUT,B-520-OUT;n:type:ShaderForge.SFN_Add,id:5665,x:33682,y:33030,varname:node_5665,prsc:2|A-2454-UVOUT,B-8644-OUT;n:type:ShaderForge.SFN_Divide,id:5149,x:32898,y:33056,cmnt:除以一个distance 离中心越远的uv,varname:node_5149,prsc:2|A-7071-OUT,B-5290-OUT;n:type:ShaderForge.SFN_Multiply,id:520,x:33192,y:33121,varname:node_520,prsc:2|A-5149-OUT,B-131-OUT;n:type:ShaderForge.SFN_Slider,id:131,x:32826,y:33349,ptovrint:False,ptlb:Scope,ptin:_Scope,cmnt:用于再一次调整稀疏度,varname:node_131,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7.538462,max:10;proporder:9006-4778-6746-2648-6262-131;pass:END;sub:END;*/

Shader "UVOperate/CircleSineDistort" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _CenterX ("CenterX", Range(0, 1)) = 0.5
        _CenterY ("CenterY", Range(0, 1)) = 0.5
        _Scale ("Scale", Range(0, 500)) = 120.3846
        _Speed ("Speed", Range(-50, 50)) = 0
        _Scope ("Scope", Range(0, 10)) = 7.538462
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _CenterX;
            uniform float _CenterY;
            uniform float _Scale;
            uniform float _Speed;
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
                float2 node_5725 = float2(_CenterX,_CenterY);
                float node_5290 = (distance(i.uv0,node_5725)*_Scale);
                float4 node_2166 = _Time + _TimeEditor;
                float node_520 = ((sin((node_5290+(_Speed*node_2166.g)))/node_5290)*_Scope);
                float2 node_5665 = (i.uv0+float2(node_520,node_520));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5665, _MainTex));
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
