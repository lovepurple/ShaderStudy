Shader "URP/URP_MultiLight" {
	Properties 
	{
		_MainTex("Main Texture",2D)="white"{}
		_MainColor("Main Color",Color) = (1,1,1,1)
		_SpecColor("Spec Color",Color) = (1.5,1,1,1)
		_SpecPower("Spec Power",Float) = 0
		[KeywordEnum(ON,OFF)]_AllowMultiLight("Allow MultiLight",Float)=1
	}
	SubShader 
	{
		Tags 
		{
			"RenderType"="Opaque"
			"IgnorePorjector"="Ture"
			"RenderPipeline" = "UnversalPipeline"
			"Queue" = "Geometry"
		}

		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\Color.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Lighting.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"
		ENDHLSL

		Pass 
		{
			Tags 
			{
				"LightMode"="UniversalForward"
			}
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _ALLOWMULTILIGHT_ON _ALLOWMULTILIGHT_OFF

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			CBUFFER_START(UnityPerMaterial)
			float4 _MainColor;
			float4 _SpecColor;
			float _SpecPower;
			CBUFFER_END

			struct a2v 
			{
				float4 positionOS : POSITION;
				float3 normalOS: NORMAL;
				float2 uv:TEXCOORD0; 
			};

			struct v2f 
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float2 uv:TEXCOORD3;
			};

			v2f vert (a2v i) 
			{
				v2f o = (v2f)0;
				o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
				o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
				o.normalWS = TransformObjectToWorldNormal(i.normalOS);
				o.uv = i.uv;
				return o;
			}

			float4 frag(v2f i) : SV_TARGET 
			{
				//光照模型： 主光Half Lambert + BlinnPhong  + 额外光 Half Lambert
				float3 baseColor = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv) * _MainColor.rgb;
				Light mainLightInfo = GetMainLight();
				float3 viewDirWS = GetWorldSpaceViewDir(i.positionWS);
				float NDL = max(0,dot(i.normalWS,mainLightInfo.direction));
				baseColor = baseColor * mainLightInfo.color * (NDL *0.5f +0.5f);

				float3 h = normalize(viewDirWS + mainLightInfo.direction);
				float HDN = saturate(dot(h,i.normalWS));
				float3 specColor = _SpecColor * pow(HDN,_SpecPower);		

				//叠加其他点光		
				float3 additionalLightCol = float3(0,0,0);

				#if _ALLOWMULTILIGHT_ON
					//叠加所有额外光
					int additionLightCount = GetAdditionalLightsCount();
					for(int lightIndex = 0 ; lightIndex< additionLightCount;++lightIndex){
						//额外光需要传入positionWS，计算点的方向 和 衰减
						Light additionLight = GetAdditionalLight(lightIndex,i.positionWS);
						float3 normalizeLightDir = normalize(additionLight.direction);
						float NDL = max(0,dot(i.normalWS,normalizeLightDir));

						//灯光衰减
						additionalLightCol += NDL * additionLight.color * additionLight.distanceAttenuation;
					}
				#endif

				
				return float4(specColor+baseColor+additionalLightCol,1.0);
			}
			ENDHLSL
		}
	}
}
