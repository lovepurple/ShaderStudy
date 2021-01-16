Shader "Shader/Other" {
    Properties {
        _MainTexture ("MainTexture", 2D) = "white" {}
        [NoScaleOffset]_Ramp ("Ramp", 2D) = "white" {}
        _RamRange("RampRange",Range(0,0.2)) = 0
        _SkinBaseColor("SkinBaseColor" , Color) = (1,1,1,1)

        _Tint1 ("Tint1", Color) = (0.5,0.5,0.5,1)
        _Tint2 ("Tint2", Color) = (0.5,0.5,0.5,1)
        [Header(Light)]
        _LightDir("LightDir",Vector) = (1,1,1,1)
        _LightColor("LightColor",Color) = (1,1,1,1)
        [Header(Skin2)]
        _SkinGlossRange ("SkinGlossRange", Range(0, 5)) = 0
        _SkinGlossPower ("SkinGlossPower", Range(0, 1)) = 0
        _GlossColor1 ("GlossColor1", Color) = (0.5,0.5,0.5,1)
        _SkinGlossRange2 ("SkinGlossRange2", Range(0, 5)) = 0
        _SkinGlossPower2 ("SkinGlossPower2", Range(0, 5)) = 0
        _SkinGlossColor2 ("SkinGlossColor2", Color) = (0.5,0.5,0.5,1)
        [Header(Fresnel)]
        _FresnelRange ("FresnelRange", Range(1, 10)) = 5
        _FresnelColor ("FresnelColor", Color) = (0.5,0.5,0.5,1)
        _FresnelPower ("FresnelPower", Range(0, 20)) = 0
        [Header(Metal)]
        _MaskTexture ("MaskTexture(Skin_R)(Cloth_G)(Metal_B)", 2D) = "white" {}
        _MetalGloss ("MetalGloss", Range(0, 1)) = 0
        _MetalGlossPower ("MetalGlossPower", Range(0, 5)) = 0
        [NoScaleOffset]_MetalGlossCube ("MetalGlossCube", Cube) = "_Skybox" {}
        _MatelGlossColor ("MatelGlossColor", Color) = (0.5,0.5,0.5,1)
        [Header(Cloth)]
        _ClothGlossRange ("ClothGlossRange", Range(0, 5)) = 0
        _ClothGlossPowerRange ("ClothGlossPowerRange", Range(0, 1)) = 0
        _ClothGloss2("ClothGloss2",Range(0,1))=0
        _ClothGloss2PowerRange("ClothGloss2PowerRange",Range(0,1)) = 0
        _ClothColor ("ClothColor", Color) = (0.5,0.5,0.5,1)
        _ClothColor2 ("ClothColor2", Color) = (0.5,0.5,0.5,1)
        _ClothMask ("ClothMask", 2D) = "white" {}
        _ClothMaskOffset("ClothMaskOffset", Vector) = (1,1,1,1)
        _Range ("Range", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }

        cull off
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            half _SkinGlossRange,_SkinGlossPower;
            sampler2D _MainTexture,_Ramp,_MaskTexture,_ClothMask;  
            half4 _MainTexture_ST,_Ramp_ST,_MaskTexture_ST,_ClothMask_ST;
            half4 _Tint1,_Tint2,_LightDir,_LightColor;
            half _FresnelRange,_FresnelPower;
            fixed _RamRange;
            float4 _SkinBaseColor;
            half4 _FresnelColor;
            half4 _GlossColor1;
            half _MetalGloss,_MetalGlossPower;
            samplerCUBE _MetalGlossCube;
            half4 _MatelGlossColor;
            half _ClothGlossRange,_ClothGlossPowerRange,_ClothGloss2,_ClothGloss2PowerRange;
            half4 _ClothColor,_ClothColor2,_ClothMaskOffset;
            half _Range,_SkinGlossRange2,_SkinGlossPower2;
            half4 _SkinGlossColor2;
            struct VertexInput {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half4 tangent : TANGENT;
                half2 texcoord : TEXCOORD0;
            };
            struct VertexOutput {
                half4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                half4 posWorld : TEXCOORD1;
                half3 normalDir : TEXCOORD2;
                half3 tangentDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, half4( v.tangent.xyz, 0 ) ).xyz );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            half4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                half3 normalDirection = i.normalDir;
                half3 viewReflectDirection = reflect( viewDirection, normalDirection );
                // half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                half3 lightDirection = normalize(_LightDir.rgb);
                half3 lightColor =_LightColor.rgb;
                half3 halfDirection = normalize(viewDirection+lightDirection);
                
                float4 _MainTexture_var = tex2D(_MainTexture,TRANSFORM_TEX(i.uv, _MainTexture))*_SkinBaseColor;
                half LBT = ((max(0,dot(lightDirection,i.normalDir))*.4)+.4);
                float3 MainTexVar = (_MainTexture_var.rgb*LBT*1.3);
                half BLGloss = max(0,dot(halfDirection,i.normalDir));
                half3 MetalGloss = (pow(BLGloss,exp2((_MetalGloss*10)))*_MetalGlossPower*texCUBE(_MetalGlossCube,viewReflectDirection).rgb*_MatelGlossColor.rgb);

                half4 _MaskTexture_var = tex2D(_MaskTexture,TRANSFORM_TEX(i.uv, _MaskTexture));
                //fixed4 _MaskTexture_var = fixed4(1,1,1,1);

                half3 Fresnel = (saturate((pow((1 - max(0,dot(lightDirection,i.normalDir))),2.0)-0.5))*pow(1-max(0,dot(normalDirection, viewDirection)),_FresnelRange)*_FresnelColor.rgb*_FresnelPower);
                
                fixed2 RampUV = half2(LBT,LBT);
                fixed2 moveUV= fixed2(0,_RamRange);
                half4 _RampVar = tex2D(_Ramp,TRANSFORM_TEX(RampUV+moveUV, _Ramp));

                //half H = normalize(cross(i.normalDir,i.tangentDir));
                half dotTH = ( dot(halfDirection,cross(i.normalDir,i.tangentDir)));
                half3 skinGlossPoint = (pow(BLGloss,exp2((_SkinGlossRange2*10)))*_SkinGlossPower2*_SkinGlossColor2.rgb);
                half3 Gloss = (skinGlossPoint+((pow(sqrt((1-(dotTH*dotTH))),(_SkinGlossRange*100))*smoothstep( (-1), 0, dotTH )*_SkinGlossPower)*_GlossColor1.rgb));

                half3 ClothGloss3 =(pow(BLGloss,exp2((_ClothGloss2*10)))*_ClothGloss2PowerRange);

               // half3 metallerp = lerp(MainTexVar,MetalGloss,_MaskTexture_var.b);
                half3 Metal =lerp(MainTexVar,MetalGloss+MainTexVar,_MaskTexture_var.b);
                half3 skinf = ((saturate((1-(1-_Tint1.rgb)*(1-_RampVar.r)))*_Tint1.a)+((1 - _Tint1.a)));
                half3 skinb = ((saturate((1-(1-_Tint2.rgb)*(1-_RampVar.g)))*_Tint2.a)+((1 - _Tint2.a)));
                float3 SkinVar = (_MainTexture_var.rgb*skinf*skinb);
                half3 Skin = (Fresnel+SkinVar+Gloss);
                half3 SkinTintOther = lerp(Metal,Skin,_MaskTexture_var.r); // 皮肤与其他区分
                half3 ClothGlossRainbow = (saturate(3*abs(1-2.0*frac(saturate(pow((1 - max(0,dot(viewDirection,i.normalDir))),_Range))+half3(0,-1/3.0,1/3.0)))-1));
                half4 _ClothMaskadd =  tex2D(_ClothMask,TRANSFORM_TEX(i.uv*_ClothMaskOffset.xy+_ClothMaskOffset.zw ,_ClothMask));
                half4 _ClothMaskColorMask =  tex2D(_ClothMask,TRANSFORM_TEX(i.uv ,_ClothMask));
                half3 ColthGlossRange = (pow(sqrt((1-(dotTH*dotTH))),(_ClothGlossRange*100))*smoothstep((-1),0,dotTH )*_ClothGlossPowerRange) +ClothGloss3;
                half3 RainbowOrColor = lerp(ClothGlossRainbow,lerp(_ClothColor2.rgb,_ClothColor.rgb,_ClothMaskColorMask.g),_ClothColor.a);
                half3 ClothGloss = (ColthGlossRange*RainbowOrColor);
                half3 clothlerp = lerp(0,ClothGloss,_ClothMaskadd.r);
                float3 outc = (lightColor*lerp(SkinTintOther,(SkinTintOther+clothlerp),_MaskTexture_var.g));
                return fixed4(outc,1);
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}
