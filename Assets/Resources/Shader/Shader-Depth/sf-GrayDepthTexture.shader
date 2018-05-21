// Shader created with Shader Forge v1.37
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33625,y:32992,varname:node_3138,prsc:2|emission-3183-OUT;n:type:ShaderForge.SFN_SceneDepth,id:3183,x:32759,y:32764,varname:node_3183,prsc:2;n:type:ShaderForge.SFN_ProjectionParameters,id:5843,x:32830,y:33208,varname:node_5843,prsc:2;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:6113,x:33194,y:33049,varname:node_6113,prsc:2|IN-3183-OUT,IMIN-5843-NEAR,IMAX-5843-FAR,OMIN-2610-OUT,OMAX-888-OUT;n:type:ShaderForge.SFN_Vector1,id:2610,x:32781,y:33048,varname:node_2610,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:888,x:32785,y:32940,varname:node_888,prsc:2,v1:1;pass:END;sub:END;*/

Shader "Shader Forge/sf-GrayDepthTexture" {
    Properties {
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
            uniform sampler2D _CameraDepthTexture;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                sceneZ = Linear01Depth(sceneZ);
                sceneZ = LinearEyeDepth(sceneZ);
                float node_3183 = sceneZ;
                float3 emissive = float3(node_3183,node_3183,node_3183);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
