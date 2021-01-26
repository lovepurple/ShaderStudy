/*
	主光源的阴影计算流程
*/

Shader "URP/URP_MainLightShadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor("MainColor",Color) = (1,1,1,1)

		_MetallicTex("Metallic Texture(Alpha Channel)",2D) = "black"{}
		_Roughtness("Roughtness",Range(0,1)) = 1

		_NormalTex("Normal Texture",2D)="white"{}
		_AOTex("Ambient Occlusion Texture",2D)="white"{}

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
            #include "AutoLight.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"

			#pragma target 3.0


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldPos:TEXCOORD1;
				float3 normalDir:TEXCOORD2;
				float3 tangentDir:TEXCOORD3;
				float3 binormalDir:TEXCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainColor;

			sampler2D _AOTex;
            float4 _AOTex_ST;
			sampler2D _NormalTex;
            float4 _NormalTex_ST;
			sampler2D _MetallicTex;
            float4 _MetallicTex_ST;
			float _Roughtness;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.tangentDir = UnityObjectToWorldDir(v.tangent.xyz);
				o.binormalDir =normalize(cross(o.normalDir,o.tangentDir) * v.tangent.w);

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				i.normalDir = normalize(i.normalDir);
				float3x3 tbnMatrix = float3x3(i.tangentDir,i.binormalDir,i.normalDir);
				float3 viewDir =normalize(UnityWorldSpaceViewDir(i.worldPos.xyz));
				float3 lightDir =UnityWorldSpaceLightDir(i.worldPos.xyz);
				float3 normalDir =UnpackNormal(tex2D(_NormalTex,TRANSFORM_TEX(i.uv,_NormalTex)));
				float3 viewReflectionDir = reflect(-viewDir,normalDir);

				float4 baseColor = tex2D(_MainTex,TRANSFORM_TEX(i.uv,_MainTex));
				float4 metallicColor = tex2D(_MetallicTex,TRANSFORM_TEX(i.uv,_MetallicTex));

				//需要使用HalfVector
				float3 halfVector = normalize(viewDir + lightDir);

				//注意这里TBN的顺序，Unity CG里的mul 可能是override了 如果向量在左面，向量就是行向量
				normalDir = normalize(mul(normalDir,tbnMatrix));

				float3 lightColor =_LightColor0;

				//光计算
				float lightAttenuation = LIGHT_ATTENUATION(i);
				lightColor = lightColor * lightAttenuation;


				//Gloss
				//Roughtness 与 Gloss是相反的
				//可以直接在_MetallicTex里的Alpha通道指定
				float gloss = 1.0 -_Roughtness;
				//真正粗糙度的计算
				float roughness = _Roughtness * _Roughtness;

				//GI计算（PBR里非常重要）
				UnityLight light;
				light.color =lightColor;
				light.dir = lightDir;
				light.ndotl = max(0,dot(normalDir,lightDir));		//也可以直接使用LambertTerm (half3 normal, half3 lightDir)

				UnityGIInput giInput;
				giInput.light = light;
				giInput.worldPos = i.worldPos;
				giInput.worldViewDir =viewDir;
				giInput.atten = lightAttenuation;

				//各种内置光照
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    giInput.boxMin[0] = unity_SpecCube0_BoxMin;
                    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
                    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
                    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
                    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                giInput.probeHDR[0] = unity_SpecCube0_HDR;
                giInput.probeHDR[1] = unity_SpecCube1_HDR;

                //天空反射探头的反射（也可不参与GI计算）
                Unity_GlossyEnvironmentData gi_environment;
                gi_environment.roughness = _Roughtness;		//这几个地方用的_Roughtness不一样
                gi_environment.reflUVW =viewReflectionDir;

                //GI结果,返回经过GI处理的光方向及颜色
                //整个Unity GI计算的核心函数
                //UnityGI 里包括直接与间接信息
                UnityGI gi = UnityGlobalIllumination(giInput,1,normalDir,gi_environment);
                lightDir = gi.light.dir;
                lightColor = gi.light.color;



                //以上全是PBR之间的准备阶段
                //PBR 高光流程
                float NDotL =saturate(dot(normalDir,lightDir));
                float LDotH =saturate(dot(lightDir,halfVector));

                //金属通道的r做为金属的纹理
                float3 specularColor = metallicColor.r;

                //用于计算高光
                float3 diffuseColor = baseColor.rgb * _MainColor.rgb;

                //高光颜色计算的部分（但Unity返回的是1-）
                //计算流程跟Standard.shader里的计算方式一致，metallicStepup
                float specularMonoChrome;
                diffuseColor =DiffuseAndSpecularFromMetallic(diffuseColor,metallicColor.r,specularColor,specularMonoChrome);

                //Cook-Torrance BRDF
                float NDotV = abs(dot(normalDir,viewDir));
                float NDotH = saturate(dot(normalDir,halfVector));
                float HDotV = saturate(dot(halfVector,viewDir));

                //DFG

                //G（遮蔽）
                float visTerm = SmithJointGGXVisibilityTerm(NDotL,NDotV,roughness);
                //D
                float normalTerm = GGXTerm(NDotH,roughness);

                //G D确定可不可见及分散情况
                //Torrance-Sparrow模型
                float specularPBL = visTerm * normalTerm * UNITY_PI;

                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif

                specularPBL = max(0,specularPBL* NDotL);
                specularPBL *= any(specularColor) ? 1.0 : 0.0;

                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*_Roughtness;
                #else
                    surfaceReduction = 1.0/(roughtness*roughtness + 1.0);
                #endif

                //最终的高光
                float3 directSpecular = lightColor * specularPBL * FresnelTerm(specularColor,LDotH);

                half grazingTerm = saturate(gloss + specularMonoChrome);
                float3 indirectSpecular =gi.indirect.specular;
                indirectSpecular *=FresnelLerp(specularColor,grazingTerm,NDotV);
                indirectSpecular *= surfaceReduction;

                float3 finalSpecular =directSpecular + indirectSpecular;

                //Diffuse计算，直接使用Disney
                NDotL = max(0,dot(normalDir,lightDir));

                //Disney 模型，一定要乘个NDotL
                float diffuseTerm = DisneyDiffuse(NDotV,NDotL,LDotH,_Roughtness) * NDotL;
                float3 directdiffuse =diffuseTerm*lightColor;

                float3 indirectdiffuse = float3(0,0,0);
                indirectdiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb;
                indirectdiffuse *= tex2D(_AOTex,i.uv).r;
                float3 diffuse = (directdiffuse + indirectdiffuse) * diffuseColor;

                float3 finalColor = diffuse + finalSpecular;

				return float4(finalColor,1.0);
			}
			ENDCG
		}
	}
}
