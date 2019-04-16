// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33549,y:33771,cmnt:使用Aspect 消除长宽比不等于11的问题,varname:node_2865,prsc:2|emission-4553-OUT;n:type:ShaderForge.SFN_Slider,id:879,x:31824,y:33818,ptovrint:False,ptlb:CircleCenterX,ptin:_CircleCenterX,varname:node_879,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:2032,x:31824,y:33922,ptovrint:False,ptlb:CircleCenterY,ptin:_CircleCenterY,varname:node_2032,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Append,id:1951,x:32199,y:33855,varname:node_1951,prsc:2|A-879-OUT,B-2032-OUT;n:type:ShaderForge.SFN_Slider,id:6052,x:32769,y:34003,ptovrint:False,ptlb:CircleRadius,ptin:_CircleRadius,varname:node_6052,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:0.5;n:type:ShaderForge.SFN_Distance,id:7127,x:32699,y:33645,varname:node_7127,prsc:2|A-8617-OUT,B-994-OUT;n:type:ShaderForge.SFN_Step,id:4553,x:33147,y:33864,varname:node_4553,prsc:2|A-7127-OUT,B-6052-OUT;n:type:ShaderForge.SFN_ScreenParameters,id:5475,x:31630,y:33505,varname:node_5475,prsc:2;n:type:ShaderForge.SFN_Divide,id:6818,x:31841,y:33505,cmnt:aspect,varname:node_6818,prsc:2|A-5475-PXW,B-5475-PXH;n:type:ShaderForge.SFN_TexCoord,id:5486,x:32110,y:33347,varname:node_5486,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8617,x:32395,y:33445,cmnt:uv coord correct,varname:node_8617,prsc:2|A-5486-UVOUT,B-4108-OUT;n:type:ShaderForge.SFN_Append,id:4108,x:32206,y:33551,varname:node_4108,prsc:2|A-6818-OUT,B-3111-OUT;n:type:ShaderForge.SFN_Vector1,id:3111,x:31978,y:33615,varname:node_3111,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:994,x:32414,y:33772,cmnt:center ,varname:node_994,prsc:2|A-4108-OUT,B-1951-OUT;proporder:879-2032-6052;pass:END;sub:END;*/

Shader "Shader Forge/DrawCircleByUVCoordWithAspect" {
    Properties {
        _CircleCenterX ("CircleCenterX", Range(0, 1)) = 0.5
        _CircleCenterY ("CircleCenterY", Range(0, 1)) = 0.5
        _CircleRadius ("CircleRadius", Range(0, 0.5)) = 0.3
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Geometry+1"
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
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _CircleCenterX;
            uniform float _CircleCenterY;
            uniform float _CircleRadius;
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
                float2 node_4108 = float2((_ScreenParams.r/_ScreenParams.g),1.0);
                float node_4553 = step(distance((i.uv0*node_4108),(node_4108*float2(_CircleCenterX,_CircleCenterY))),_CircleRadius);
                float3 emissive = float3(node_4553,node_4553,node_4553);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
