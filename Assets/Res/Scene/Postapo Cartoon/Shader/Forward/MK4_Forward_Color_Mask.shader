// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:33840,y:32936,varname:node_2865,prsc:2|diff-6861-OUT,spec-2615-R,gloss-3389-OUT,normal-5964-RGB,difocc-3116-A,spcocc-3116-A,olwid-9628-OUT,olcol-8690-RGB;n:type:ShaderForge.SFN_Multiply,id:6343,x:31971,y:32493,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31783,y:32593,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31587,y:32487,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:edaa351d853074342a5a40ddbf7306f9,ntxv:0,isnm:False|UVIN-8113-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:33092,y:33041,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:bfcb3b51947e1d548b505a82adeda304,ntxv:3,isnm:True;n:type:ShaderForge.SFN_TexCoord,id:6392,x:31110,y:32917,varname:node_6392,prsc:2,uv:1;n:type:ShaderForge.SFN_TexCoord,id:8113,x:31110,y:32730,varname:node_8113,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:2615,x:31582,y:32837,varname:node_2615,prsc:2,tex:e8cca8fbfa6364044823ebfd7beb51a4,ntxv:0,isnm:False|UVIN-8113-UVOUT,TEX-5348-TEX;n:type:ShaderForge.SFN_Tex2d,id:4754,x:32520,y:32541,ptovrint:False,ptlb:Dirt,ptin:_Dirt,varname:node_4754,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5a9229c780de65e45adc621f388094c7,ntxv:0,isnm:False|UVIN-8113-UVOUT;n:type:ShaderForge.SFN_Multiply,id:633,x:32762,y:32558,varname:node_633,prsc:2|A-4754-RGB,B-4348-OUT;n:type:ShaderForge.SFN_Slider,id:5745,x:30728,y:31885,ptovrint:False,ptlb:Leaks scale,ptin:_Leaksscale,varname:node_1555,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_RemapRange,id:2107,x:31089,y:31901,varname:node_2107,prsc:2,frmn:0,frmx:1,tomn:0.05,tomx:0.5|IN-5745-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:1057,x:30935,y:31622,varname:node_1057,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5594,x:31332,y:31593,varname:node_5594,prsc:2|A-1057-X,B-2107-OUT;n:type:ShaderForge.SFN_Multiply,id:828,x:31332,y:31708,varname:node_828,prsc:2|A-1057-Y,B-2107-OUT;n:type:ShaderForge.SFN_Multiply,id:494,x:31332,y:31827,varname:node_494,prsc:2|A-1057-Z,B-2107-OUT;n:type:ShaderForge.SFN_Append,id:4126,x:31715,y:31829,varname:node_4126,prsc:2|A-5594-OUT,B-828-OUT;n:type:ShaderForge.SFN_Append,id:7145,x:31715,y:31571,varname:node_7145,prsc:2|A-494-OUT,B-828-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:5167,x:31879,y:31718,ptovrint:False,ptlb:Leaks,ptin:_Leaks,varname:node_7599,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:76163e355df5e8b47878ecb2511ecd98,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:77,x:32103,y:31562,varname:node_1538,prsc:2,tex:76163e355df5e8b47878ecb2511ecd98,ntxv:0,isnm:False|UVIN-7145-OUT,TEX-5167-TEX;n:type:ShaderForge.SFN_Tex2d,id:7266,x:32118,y:31815,varname:node_8935,prsc:2,tex:76163e355df5e8b47878ecb2511ecd98,ntxv:0,isnm:False|UVIN-4126-OUT,TEX-5167-TEX;n:type:ShaderForge.SFN_NormalVector,id:8711,x:31418,y:32009,prsc:2,pt:True;n:type:ShaderForge.SFN_Abs,id:721,x:31664,y:31972,varname:node_721,prsc:2|IN-8711-OUT;n:type:ShaderForge.SFN_Multiply,id:7850,x:31918,y:32068,varname:node_7850,prsc:2|A-721-OUT,B-721-OUT;n:type:ShaderForge.SFN_ChannelBlend,id:1028,x:32468,y:31735,varname:node_1028,prsc:2,chbt:0|M-7850-OUT,R-77-RGB,G-7800-OUT,B-7266-RGB;n:type:ShaderForge.SFN_Vector3,id:7800,x:32103,y:31467,varname:node_7800,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Multiply,id:6861,x:33196,y:32372,varname:node_6861,prsc:2|A-1028-OUT,B-633-OUT;n:type:ShaderForge.SFN_OneMinus,id:300,x:32787,y:32696,varname:node_300,prsc:2|IN-4754-R;n:type:ShaderForge.SFN_Subtract,id:4272,x:32926,y:32759,varname:node_4272,prsc:2|A-300-OUT,B-6829-OUT;n:type:ShaderForge.SFN_Vector1,id:6829,x:32762,y:32845,varname:node_6829,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Add,id:8133,x:33143,y:32851,varname:node_8133,prsc:2|A-4272-OUT,B-2615-G;n:type:ShaderForge.SFN_OneMinus,id:4094,x:33303,y:32589,varname:node_4094,prsc:2|IN-1028-OUT;n:type:ShaderForge.SFN_Add,id:3389,x:33384,y:32753,varname:node_3389,prsc:2|A-186-OUT,B-8133-OUT;n:type:ShaderForge.SFN_ComponentMask,id:186,x:33475,y:32560,varname:node_186,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-4094-OUT;n:type:ShaderForge.SFN_Lerp,id:4348,x:32306,y:32485,varname:node_4348,prsc:2|A-6343-OUT,B-3842-RGB,T-2615-B;n:type:ShaderForge.SFN_Color,id:3842,x:32057,y:32713,ptovrint:False,ptlb:Paint,ptin:_Paint,varname:node_3842,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5294118,c2:0.1206747,c3:0.1206747,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:5348,x:31377,y:32935,ptovrint:False,ptlb:Masks (RG) Metal/Gloss (B)Mask (A)AO,ptin:_MasksRGMetalGlossBMaskAAO,varname:node_5348,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e8cca8fbfa6364044823ebfd7beb51a4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3116,x:31582,y:32980,varname:node_3116,prsc:2,tex:e8cca8fbfa6364044823ebfd7beb51a4,ntxv:0,isnm:False|UVIN-6392-UVOUT,TEX-5348-TEX;n:type:ShaderForge.SFN_Slider,id:9315,x:33178,y:33357,ptovrint:False,ptlb:Outline,ptin:_Outline,varname:node_4294,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_RemapRange,id:9628,x:33568,y:33407,varname:node_9628,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.1|IN-9315-OUT;n:type:ShaderForge.SFN_Color,id:8690,x:33568,y:33588,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:_OutlineColor_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;proporder:7736-6665-5964-5348-3842-4754-5167-5745-9315-8690;pass:END;sub:END;*/

Shader "MK4/Forward/MK4_Color_Mask" {
    Properties {
        _MainTex ("Base Color", 2D) = "white" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MasksRGMetalGlossBMaskAAO ("Masks (RG) Metal/Gloss (B)Mask (A)AO", 2D) = "white" {}
        _Paint ("Paint", Color) = (0.5294118,0.1206747,0.1206747,1)
        _Dirt ("Dirt", 2D) = "white" {}
        _Leaks ("Leaks", 2D) = "white" {}
        _Leaksscale ("Leaks scale", Range(0, 1)) = 0.2
        _Outline ("Outline", Range(0, 1)) = 0.2
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _Outline;
            uniform float4 _OutlineColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal*(_Outline*0.1+0.0),1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                return fixed4(_OutlineColor.rgb,0);
            }
            ENDCG
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _Dirt; uniform float4 _Dirt_ST;
            uniform float _Leaksscale;
            uniform sampler2D _Leaks; uniform float4 _Leaks_ST;
            uniform float4 _Paint;
            uniform sampler2D _MasksRGMetalGlossBMaskAAO; uniform float4 _MasksRGMetalGlossBMaskAAO_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float3 node_721 = abs(normalDirection);
                float3 node_7850 = (node_721*node_721);
                float node_2107 = (_Leaksscale*0.45+0.05);
                float node_828 = (i.posWorld.g*node_2107);
                float2 node_7145 = float2((i.posWorld.b*node_2107),node_828);
                float4 node_1538 = tex2D(_Leaks,node_7145);
                float2 node_4126 = float2((i.posWorld.r*node_2107),node_828);
                float4 node_8935 = tex2D(_Leaks,node_4126);
                float3 node_1028 = (node_7850.r*node_1538.rgb + node_7850.g*float3(1,1,1) + node_7850.b*node_8935.rgb);
                float4 _Dirt_var = tex2D(_Dirt,TRANSFORM_TEX(i.uv0, _Dirt));
                float4 node_2615 = tex2D(_MasksRGMetalGlossBMaskAAO,i.uv0);
                float gloss = ((1.0 - node_1028).r+(((1.0 - _Dirt_var.r)-0.2)+node_2615.g));
                float specPow = exp2( gloss * 10.0+1.0);
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
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 node_3116 = tex2D(_MasksRGMetalGlossBMaskAAO,i.uv1);
                float3 specularAO = node_3116.a;
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (node_1028*(_Dirt_var.rgb*lerp((_MainTex_var.rgb*_Color.rgb),_Paint.rgb,node_2615.b))); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, node_2615.r, specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4 );
                float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular) * specularAO;
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                indirectDiffuse *= node_3116.a; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _Dirt; uniform float4 _Dirt_ST;
            uniform float _Leaksscale;
            uniform sampler2D _Leaks; uniform float4 _Leaks_ST;
            uniform float4 _Paint;
            uniform sampler2D _MasksRGMetalGlossBMaskAAO; uniform float4 _MasksRGMetalGlossBMaskAAO_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float3 node_721 = abs(normalDirection);
                float3 node_7850 = (node_721*node_721);
                float node_2107 = (_Leaksscale*0.45+0.05);
                float node_828 = (i.posWorld.g*node_2107);
                float2 node_7145 = float2((i.posWorld.b*node_2107),node_828);
                float4 node_1538 = tex2D(_Leaks,node_7145);
                float2 node_4126 = float2((i.posWorld.r*node_2107),node_828);
                float4 node_8935 = tex2D(_Leaks,node_4126);
                float3 node_1028 = (node_7850.r*node_1538.rgb + node_7850.g*float3(1,1,1) + node_7850.b*node_8935.rgb);
                float4 _Dirt_var = tex2D(_Dirt,TRANSFORM_TEX(i.uv0, _Dirt));
                float4 node_2615 = tex2D(_MasksRGMetalGlossBMaskAAO,i.uv0);
                float gloss = ((1.0 - node_1028).r+(((1.0 - _Dirt_var.r)-0.2)+node_2615.g));
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (node_1028*(_Dirt_var.rgb*lerp((_MainTex_var.rgb*_Color.rgb),_Paint.rgb,node_2615.b))); // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, node_2615.r, specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4 );
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Dirt; uniform float4 _Dirt_ST;
            uniform float _Leaksscale;
            uniform sampler2D _Leaks; uniform float4 _Leaks_ST;
            uniform float4 _Paint;
            uniform sampler2D _MasksRGMetalGlossBMaskAAO; uniform float4 _MasksRGMetalGlossBMaskAAO_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float3 node_721 = abs(normalDirection);
                float3 node_7850 = (node_721*node_721);
                float node_2107 = (_Leaksscale*0.45+0.05);
                float node_828 = (i.posWorld.g*node_2107);
                float2 node_7145 = float2((i.posWorld.b*node_2107),node_828);
                float4 node_1538 = tex2D(_Leaks,node_7145);
                float2 node_4126 = float2((i.posWorld.r*node_2107),node_828);
                float4 node_8935 = tex2D(_Leaks,node_4126);
                float3 node_1028 = (node_7850.r*node_1538.rgb + node_7850.g*float3(1,1,1) + node_7850.b*node_8935.rgb);
                float4 _Dirt_var = tex2D(_Dirt,TRANSFORM_TEX(i.uv0, _Dirt));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_2615 = tex2D(_MasksRGMetalGlossBMaskAAO,i.uv0);
                float3 diffColor = (node_1028*(_Dirt_var.rgb*lerp((_MainTex_var.rgb*_Color.rgb),_Paint.rgb,node_2615.b)));
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, node_2615.r, specColor, specularMonochrome );
                float roughness = 1.0 - ((1.0 - node_1028).r+(((1.0 - _Dirt_var.r)-0.2)+node_2615.g));
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
