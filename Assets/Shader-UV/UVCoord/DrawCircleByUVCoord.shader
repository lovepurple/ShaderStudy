// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32929,y:33017,cmnt:使用UV坐标系画圆用于理解不同aspect对于i.uv的影响,varname:node_2865,prsc:2|emission-6415-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:32066,y:32837,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:2742,x:32145,y:33277,ptovrint:False,ptlb:CircleRadius,ptin:_CircleRadius,varname:node_2742,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:0.5;n:type:ShaderForge.SFN_Slider,id:1298,x:31689,y:32977,ptovrint:False,ptlb:CircleCenterX,ptin:_CircleCenterX,varname:node_1298,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:9123,x:31689,y:33105,ptovrint:False,ptlb:CircleCenterY,ptin:_CircleCenterY,varname:node_9123,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Append,id:2237,x:32066,y:33039,cmnt:circle center ,varname:node_2237,prsc:2|A-1298-OUT,B-9123-OUT;n:type:ShaderForge.SFN_Distance,id:4838,x:32302,y:33012,varname:node_4838,prsc:2|A-4219-UVOUT,B-2237-OUT;n:type:ShaderForge.SFN_Step,id:6415,x:32542,y:33107,varname:node_6415,prsc:2|A-4838-OUT,B-2742-OUT;proporder:2742-1298-9123;pass:END;sub:END;*/

Shader "UVOperate/DrawCircleByUVCoord" {
    Properties {
        _CircleRadius ("CircleRadius", Range(0, 0.5)) = 0.1
        _CircleCenterX ("CircleCenterX", Range(0, 1)) = 0.5
        _CircleCenterY ("CircleCenterY", Range(0, 1)) = 0.5
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
            uniform float _CircleRadius;
            uniform float _CircleCenterX;
            uniform float _CircleCenterY;
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
                float node_6415 = step(distance(i.uv0,float2(_CircleCenterX,_CircleCenterY)),_CircleRadius);
                float3 emissive = float3(node_6415,node_6415,node_6415);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
