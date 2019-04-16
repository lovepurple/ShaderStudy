// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33527,y:33824,varname:node_2865,prsc:2|emission-9774-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:32431,y:34001,cmnt:Default coordinates,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:7298,x:32902,y:34053,ptovrint:False,ptlb:CircleRadiusPixels,ptin:_CircleRadiusPixels,varname:node_7298,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:100;n:type:ShaderForge.SFN_Slider,id:2781,x:32068,y:33509,ptovrint:False,ptlb:CircleCenterUVCoordX,ptin:_CircleCenterUVCoordX,varname:node_2781,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:7937,x:32068,y:33675,ptovrint:False,ptlb:CircleCenterUVCoordY,ptin:_CircleCenterUVCoordY,varname:node_7937,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Append,id:6625,x:32431,y:33599,varname:node_6625,prsc:2|A-2781-OUT,B-7937-OUT;n:type:ShaderForge.SFN_ScreenParameters,id:9428,x:32209,y:33790,varname:node_9428,prsc:2;n:type:ShaderForge.SFN_Append,id:8099,x:32431,y:33790,varname:node_8099,prsc:2|A-9428-PXW,B-9428-PXH;n:type:ShaderForge.SFN_Multiply,id:78,x:32646,y:33702,cmnt:CircleCenter in pixelCoord,varname:node_78,prsc:2|A-6625-OUT,B-8099-OUT;n:type:ShaderForge.SFN_Multiply,id:5335,x:32647,y:33962,cmnt:uvcoord to pixelCoord,varname:node_5335,prsc:2|A-8099-OUT,B-4219-UVOUT;n:type:ShaderForge.SFN_Distance,id:339,x:32902,y:33826,varname:node_339,prsc:2|A-78-OUT,B-5335-OUT;n:type:ShaderForge.SFN_Step,id:9774,x:33120,y:33914,varname:node_9774,prsc:2|A-339-OUT,B-7298-OUT;proporder:7298-2781-7937;pass:END;sub:END;*/

Shader "Shader Forge/DrawCircleByPixelCoord" {
    Properties {
        _CircleRadiusPixels ("CircleRadiusPixels", Float ) = 100
        _CircleCenterUVCoordX ("CircleCenterUVCoordX", Range(0, 1)) = 0.5
        _CircleCenterUVCoordY ("CircleCenterUVCoordY", Range(0, 1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
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
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _CircleRadiusPixels;
            uniform float _CircleCenterUVCoordX;
            uniform float _CircleCenterUVCoordY;
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
                float2 node_8099 = float2(_ScreenParams.r,_ScreenParams.g);
                float node_9774 = step(distance((float2(_CircleCenterUVCoordX,_CircleCenterUVCoordY)*node_8099),(node_8099*i.uv0)),_CircleRadiusPixels);
                float3 emissive = float3(node_9774,node_9774,node_9774);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
