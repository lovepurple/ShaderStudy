// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.875,fgcb:0.9044118,fgca:1,fgde:0.01,fgrn:0,fgrf:500,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34578,y:34658,varname:node_2865,prsc:2|diff-2849-OUT,spec-358-OUT,gloss-1813-OUT,normal-5964-RGB;n:type:ShaderForge.SFN_Tex2d,id:5964,x:34431,y:33359,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:358,x:34274,y:33161,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:34274,y:33263,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_DepthBlend,id:9853,x:31255,y:31900,varname:node_9853,prsc:2|DIST-8526-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8526,x:31005,y:31902,ptovrint:False,ptlb:WaterDepth,ptin:_WaterDepth,varname:node_8526,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Color,id:4640,x:31189,y:31437,ptovrint:False,ptlb:WaterColor,ptin:_WaterColor,varname:node_4640,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:6561,x:31055,y:31691,ptovrint:False,ptlb:DeepDarkness,ptin:_DeepDarkness,varname:node_6561,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3141892,max:1;n:type:ShaderForge.SFN_Multiply,id:6718,x:31481,y:31532,cmnt:深处的水颜色更暗,varname:node_6718,prsc:2|A-4640-RGB,B-6561-OUT;n:type:ShaderForge.SFN_Lerp,id:1184,x:31778,y:31446,cmnt:水的主体颜色,varname:node_1184,prsc:2|A-6718-OUT,B-4640-RGB,T-4450-OUT;n:type:ShaderForge.SFN_OneMinus,id:4450,x:31537,y:31787,varname:node_4450,prsc:2|IN-9853-OUT;n:type:ShaderForge.SFN_Color,id:4954,x:31265,y:32235,ptovrint:False,ptlb:RimColor,ptin:_RimColor,varname:node_4954,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7470,x:30855,y:32599,ptovrint:False,ptlb:RimRange,ptin:_RimRange,varname:node_7470,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_DepthBlend,id:5609,x:31085,y:32596,varname:node_5609,prsc:2|DIST-7470-OUT;n:type:ShaderForge.SFN_Clamp01,id:3246,x:31968,y:32501,cmnt:Rim 过渡范围,varname:node_3246,prsc:2|IN-2114-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3016,x:31095,y:32784,ptovrint:False,ptlb:RimFalloff,ptin:_RimFalloff,varname:node_3016,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.8;n:type:ShaderForge.SFN_Lerp,id:9510,x:32201,y:32084,cmnt:添加Rim,varname:node_9510,prsc:2|A-4954-RGB,B-1184-OUT,T-3246-OUT;n:type:ShaderForge.SFN_Power,id:9392,x:31334,y:32663,varname:node_9392,prsc:2|VAL-5609-OUT,EXP-3016-OUT;n:type:ShaderForge.SFN_NormalVector,id:7418,x:31766,y:34331,prsc:2,pt:True;n:type:ShaderForge.SFN_Fresnel,id:9759,x:32054,y:34331,varname:node_9759,prsc:2|NRM-7418-OUT;n:type:ShaderForge.SFN_Color,id:4842,x:32174,y:34809,ptovrint:False,ptlb:FresnelColor,ptin:_FresnelColor,varname:node_4842,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:877,x:32504,y:34581,varname:node_877,prsc:2|A-988-OUT,B-4842-RGB;n:type:ShaderForge.SFN_ValueProperty,id:1759,x:32057,y:34573,ptovrint:False,ptlb:FresnelPower,ptin:_FresnelPower,varname:node_1759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Power,id:988,x:32283,y:34434,varname:node_988,prsc:2|VAL-9759-OUT,EXP-1759-OUT;n:type:ShaderForge.SFN_Lerp,id:2849,x:33311,y:33120,varname:node_2849,prsc:2|A-9510-OUT,B-877-OUT,T-988-OUT;n:type:ShaderForge.SFN_Multiply,id:3256,x:31559,y:32499,varname:node_3256,prsc:2|A-4954-A,B-9392-OUT;n:type:ShaderForge.SFN_Slider,id:5413,x:29249,y:31236,ptovrint:False,ptlb:Tilling,ptin:_Tilling,varname:node_5413,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_TexCoord,id:1311,x:29393,y:31379,varname:node_1311,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:3670,x:29740,y:31294,varname:node_3670,prsc:2|A-5413-OUT,B-1311-UVOUT;n:type:ShaderForge.SFN_Set,id:2700,x:29940,y:31295,varname:MainTilling,prsc:2|IN-3670-OUT;n:type:ShaderForge.SFN_Time,id:6110,x:29376,y:31658,varname:node_6110,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:2448,x:29371,y:31921,ptovrint:False,ptlb:WaveSpeed,ptin:_WaveSpeed,varname:node_2448,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:5449,x:29673,y:31743,varname:node_5449,prsc:2|A-6110-T,B-2448-OUT;n:type:ShaderForge.SFN_Set,id:3606,x:30066,y:31529,varname:WaveSpeed,prsc:2|IN-5449-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7820,x:30904,y:33244,ptovrint:False,ptlb:FoamTex,ptin:_FoamTex,varname:node_7820,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9171,x:31347,y:33071,varname:node_9171,prsc:2,ntxv:0,isnm:False|UVIN-7011-UVOUT,TEX-7820-TEX;n:type:ShaderForge.SFN_Tex2d,id:4583,x:31347,y:33433,varname:node_4583,prsc:2,ntxv:0,isnm:False|UVIN-2739-UVOUT,TEX-7820-TEX;n:type:ShaderForge.SFN_ValueProperty,id:2466,x:29995,y:33210,ptovrint:False,ptlb:ExtraFoamTiling,ptin:_ExtraFoamTiling,varname:node_2466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Get,id:9264,x:29996,y:33362,varname:node_9264,prsc:2|IN-2700-OUT;n:type:ShaderForge.SFN_Multiply,id:3844,x:30222,y:33252,varname:node_3844,prsc:2|A-2466-OUT,B-9264-OUT;n:type:ShaderForge.SFN_Panner,id:7011,x:30719,y:32985,cmnt:底层泡沫,varname:node_7011,prsc:2,spu:1,spv:0|UVIN-3844-OUT,DIST-568-OUT;n:type:ShaderForge.SFN_Get,id:568,x:30501,y:33245,varname:node_568,prsc:2|IN-3606-OUT;n:type:ShaderForge.SFN_Panner,id:2739,x:30740,y:33449,cmnt:上层的泡沫流动,varname:node_2739,prsc:2,spu:0,spv:1|UVIN-3844-OUT,DIST-568-OUT;n:type:ShaderForge.SFN_Multiply,id:6400,x:31663,y:33258,cmnt:最终的泡沫流动,varname:node_6400,prsc:2|A-9171-R,B-4583-R;n:type:ShaderForge.SFN_Multiply,id:27,x:31929,y:33032,varname:node_27,prsc:2|A-3256-OUT,B-3302-OUT;n:type:ShaderForge.SFN_Add,id:2114,x:32191,y:32853,varname:node_2114,prsc:2|A-3256-OUT,B-27-OUT;n:type:ShaderForge.SFN_OneMinus,id:3302,x:31813,y:33196,varname:node_3302,prsc:2|IN-6400-OUT;proporder:5964-358-1813-8526-4640-6561-4954-7470-3016-4842-1759-5413-2448-7820-2466-9492-7067-2919-563-7812-9165;pass:END;sub:END;*/

Shader "Shader Forge/sf-PbrWater" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Specular ("Specular", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _WaterDepth ("WaterDepth", Float ) = 10
        _WaterColor ("WaterColor", Color) = (0.5,0.5,0.5,1)
        _DeepDarkness ("DeepDarkness", Range(0, 1)) = 0.3141892
        _RimColor ("RimColor", Color) = (1,1,1,1)
        _RimRange ("RimRange", Float ) = 2
        _RimFalloff ("RimFalloff", Float ) = 0.8
        _FresnelColor ("FresnelColor", Color) = (0.5,0.5,0.5,1)
        _FresnelPower ("FresnelPower", Float ) = 5
        _Tilling ("Tilling", Range(-2, 2)) = 0
        _WaveSpeed ("WaveSpeed", Float ) = 0.1
        _FoamTex ("FoamTex", 2D) = "white" {}
        _ExtraFoamTiling ("ExtraFoamTiling", Float ) = 1
        _node_574 ("node_574", 2D) = "white" {}
        _node_4910 ("node_4910", 2D) = "white" {}
        _node_7935 ("node_7935", 2D) = "white" {}
        _node_8492 ("node_8492", 2D) = "white" {}
        _node_9599 ("node_9599", 2D) = "white" {}
        _node_3554 ("node_3554", 2D) = "white" {}
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
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float _WaterDepth;
            uniform float4 _WaterColor;
            uniform float _DeepDarkness;
            uniform float4 _RimColor;
            uniform float _RimRange;
            uniform float _RimFalloff;
            uniform float4 _FresnelColor;
            uniform float _FresnelPower;
            uniform float _Tilling;
            uniform float _WaveSpeed;
            uniform sampler2D _FoamTex; uniform float4 _FoamTex_ST;
            uniform float _ExtraFoamTiling;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 projPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
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
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
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
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float specularMonochrome;
                float node_3256 = (_RimColor.a*pow(saturate((sceneZ-partZ)/_RimRange),_RimFalloff));
                float4 node_6110 = _Time + _TimeEditor;
                float WaveSpeed = (node_6110.g*_WaveSpeed);
                float node_568 = WaveSpeed;
                float2 MainTilling = (_Tilling*i.uv0);
                float2 node_3844 = (_ExtraFoamTiling*MainTilling);
                float2 node_7011 = (node_3844+node_568*float2(1,0)); // 底层泡沫
                float4 node_9171 = tex2D(_FoamTex,TRANSFORM_TEX(node_7011, _FoamTex));
                float2 node_2739 = (node_3844+node_568*float2(0,1)); // 上层的泡沫流动
                float4 node_4583 = tex2D(_FoamTex,TRANSFORM_TEX(node_2739, _FoamTex));
                float node_6400 = (node_9171.r*node_4583.r); // 最终的泡沫流动
                float node_3246 = saturate((node_3256+(node_3256*(1.0 - node_6400)))); // Rim 过渡范围
                float node_988 = pow((1.0-max(0,dot(normalDirection, viewDirection))),_FresnelPower);
                float3 diffuseColor = lerp(lerp(_RimColor.rgb,lerp((_WaterColor.rgb*_DeepDarkness),_WaterColor.rgb,(1.0 - saturate((sceneZ-partZ)/_WaterDepth))),node_3246),(node_988*_FresnelColor.rgb),node_988); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                diffuseColor *= 1-specularMonochrome;
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float _WaterDepth;
            uniform float4 _WaterColor;
            uniform float _DeepDarkness;
            uniform float4 _RimColor;
            uniform float _RimRange;
            uniform float _RimFalloff;
            uniform float4 _FresnelColor;
            uniform float _FresnelPower;
            uniform float _Tilling;
            uniform float _WaveSpeed;
            uniform sampler2D _FoamTex; uniform float4 _FoamTex_ST;
            uniform float _ExtraFoamTiling;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 projPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
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
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float specularMonochrome;
                float node_3256 = (_RimColor.a*pow(saturate((sceneZ-partZ)/_RimRange),_RimFalloff));
                float4 node_6110 = _Time + _TimeEditor;
                float WaveSpeed = (node_6110.g*_WaveSpeed);
                float node_568 = WaveSpeed;
                float2 MainTilling = (_Tilling*i.uv0);
                float2 node_3844 = (_ExtraFoamTiling*MainTilling);
                float2 node_7011 = (node_3844+node_568*float2(1,0)); // 底层泡沫
                float4 node_9171 = tex2D(_FoamTex,TRANSFORM_TEX(node_7011, _FoamTex));
                float2 node_2739 = (node_3844+node_568*float2(0,1)); // 上层的泡沫流动
                float4 node_4583 = tex2D(_FoamTex,TRANSFORM_TEX(node_2739, _FoamTex));
                float node_6400 = (node_9171.r*node_4583.r); // 最终的泡沫流动
                float node_3246 = saturate((node_3256+(node_3256*(1.0 - node_6400)))); // Rim 过渡范围
                float node_988 = pow((1.0-max(0,dot(normalDirection, viewDirection))),_FresnelPower);
                float3 diffuseColor = lerp(lerp(_RimColor.rgb,lerp((_WaterColor.rgb*_DeepDarkness),_WaterColor.rgb,(1.0 - saturate((sceneZ-partZ)/_WaterDepth))),node_3246),(node_988*_FresnelColor.rgb),node_988); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
