// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:1,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34162,y:35271,varname:node_2865,prsc:2|diff-2222-RGB,spec-8475-OUT;n:type:ShaderForge.SFN_Color,id:7078,x:31975,y:35001,ptovrint:False,ptlb:RimColor,ptin:_RimColor,varname:node_7078,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:5168,x:30275,y:35503,ptovrint:False,ptlb:RimSize,ptin:_RimSize,varname:node_5168,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Min,id:644,x:31130,y:35314,cmnt:过滤区域,varname:node_644,prsc:2|A-1931-OUT,B-1880-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1931,x:30850,y:34542,ptovrint:False,ptlb:WaterDepth,ptin:_WaterDepth,varname:node_1931,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_DepthBlend,id:626,x:31323,y:35366,varname:node_626,prsc:2|DIST-644-OUT;n:type:ShaderForge.SFN_Clamp01,id:4676,x:31536,y:35407,varname:node_4676,prsc:2|IN-626-OUT;n:type:ShaderForge.SFN_Lerp,id:4253,x:32661,y:35306,varname:node_4253,prsc:2|A-7078-RGB,B-2613-RGB,T-4676-OUT;n:type:ShaderForge.SFN_Color,id:2613,x:31687,y:34726,ptovrint:False,ptlb:WaterColor,ptin:_WaterColor,varname:node_2613,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_OneMinus,id:7656,x:32545,y:35713,varname:node_7656,prsc:2|IN-4676-OUT;n:type:ShaderForge.SFN_Vector1,id:1631,x:30292,y:35406,varname:node_1631,prsc:2,v1:0;n:type:ShaderForge.SFN_Clamp,id:1880,x:30759,y:35446,cmnt:处理小于0的情况,varname:node_1880,prsc:2|IN-5168-OUT,MIN-1631-OUT,MAX-5511-OUT;n:type:ShaderForge.SFN_Abs,id:5511,x:30543,y:35548,varname:node_5511,prsc:2|IN-5168-OUT;n:type:ShaderForge.SFN_Slider,id:8475,x:33703,y:35375,ptovrint:False,ptlb:node_8475,ptin:_node_8475,varname:node_8475,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:9694,x:33596,y:36228,ptovrint:False,ptlb:node_9694,ptin:_node_9694,varname:node_9694,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_NormalBlend,id:7246,x:33871,y:36040,varname:node_7246,prsc:2|BSE-4240-OUT,DTL-9694-RGB;n:type:ShaderForge.SFN_NormalVector,id:4240,x:33265,y:35916,prsc:2,pt:True;n:type:ShaderForge.SFN_Color,id:4523,x:33008,y:35005,ptovrint:False,ptlb:FresnelColor,ptin:_FresnelColor,varname:node_4523,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.05856987,c2:0,c3:0.7720588,c4:1;n:type:ShaderForge.SFN_Fresnel,id:3771,x:30672,y:33465,varname:node_3771,prsc:2|NRM-5847-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8587,x:30683,y:33688,ptovrint:False,ptlb:FresnelContrast,ptin:_FresnelContrast,varname:node_8587,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_NormalVector,id:5847,x:30174,y:33515,prsc:2,pt:False;n:type:ShaderForge.SFN_Power,id:8974,x:30956,y:33515,varname:node_8974,prsc:2|VAL-3771-OUT,EXP-8587-OUT;n:type:ShaderForge.SFN_Lerp,id:7502,x:33360,y:34778,varname:node_7502,prsc:2|A-2613-RGB,B-4523-RGB,T-5724-OUT;n:type:ShaderForge.SFN_Set,id:6503,x:31200,y:33523,varname:FresnelValue,prsc:2|IN-8974-OUT;n:type:ShaderForge.SFN_Get,id:5724,x:32694,y:34687,varname:node_5724,prsc:2|IN-6503-OUT;n:type:ShaderForge.SFN_Multiply,id:5135,x:33641,y:34963,varname:node_5135,prsc:2|A-7502-OUT,B-4253-OUT;n:type:ShaderForge.SFN_Lerp,id:6184,x:33452,y:35209,varname:node_6184,prsc:2|A-7502-OUT,B-4253-OUT,T-5589-OUT;n:type:ShaderForge.SFN_Slider,id:5589,x:33440,y:35632,ptovrint:False,ptlb:node_5589,ptin:_node_5589,varname:node_5589,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:8470,x:31421,y:34561,ptovrint:False,ptlb:DeepDarkness,ptin:_DeepDarkness,cmnt:深水区域更暗暗色系数,varname:node_8470,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_DepthBlend,id:4667,x:31472,y:33996,varname:node_4667,prsc:2|DIST-1931-OUT;n:type:ShaderForge.SFN_Lerp,id:1914,x:32412,y:34204,varname:node_1914,prsc:2|A-5103-OUT,B-2613-RGB,T-1344-OUT;n:type:ShaderForge.SFN_OneMinus,id:9877,x:31718,y:34001,varname:node_9877,prsc:2|IN-4667-OUT;n:type:ShaderForge.SFN_Multiply,id:5103,x:31881,y:34412,varname:node_5103,prsc:2|A-8470-OUT,B-2613-RGB;n:type:ShaderForge.SFN_Vector1,id:1344,x:32463,y:33903,varname:node_1344,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:8418,x:34035,y:34920,varname:node_8418,prsc:2|A-4523-RGB,B-5649-OUT;n:type:ShaderForge.SFN_Vector1,id:5649,x:33715,y:34766,varname:node_5649,prsc:2,v1:0;n:type:ShaderForge.SFN_Color,id:2222,x:34470,y:34845,ptovrint:False,ptlb:node_2222,ptin:_node_2222,varname:node_2222,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;proporder:2613-1931-8470-5168-7078-8475-9694-4523-8587-5589-2222;pass:END;sub:END;*/

Shader "Shader Forge/sf-pbr-water" {
    Properties {
        _WaterColor ("WaterColor", Color) = (0.5,0.5,0.5,1)
        _WaterDepth ("WaterDepth", Float ) = 0
        _DeepDarkness ("DeepDarkness", Range(0, 1)) = 0
        _RimSize ("RimSize", Float ) = 2
        _RimColor ("RimColor", Color) = (0.5,0.5,0.5,1)
        _node_8475 ("node_8475", Range(0, 1)) = 0
        _node_9694 ("node_9694", 2D) = "bump" {}
        _FresnelColor ("FresnelColor", Color) = (0.05856987,0,0.7720588,1)
        _FresnelContrast ("FresnelContrast", Float ) = 1
        _node_5589 ("node_5589", Range(0, 1)) = 0
        _node_2222 ("node_2222", Color) = (0,0,0,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float _node_8475;
            uniform float4 _node_2222;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_node_8475,_node_8475,_node_8475);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 indirectSpecular = (gi.indirect.specular)*specularColor;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = _node_2222.rgb;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
