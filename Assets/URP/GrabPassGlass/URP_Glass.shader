Shader "URP/URP_Glass"
{
	Properties
	{
		_DistortNormal("Distort Normal",2D)="bump"{}
		_DistortStrength("Distort Strength",Float) = 1
		_NoiseTex("Noise Tex",2D)="black"{}
		[KeywordEnum(On,Off)] _UseNormal("Use Normal Or Noise",Float) =1
	}

	SubShader 
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
			"LightMode"="UniversalForward"
		}

		HLSLINCLUDE
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\Core.hlsl"
		#include "Packages\com.unity.render-pipelines.core\ShaderLibrary\SpaceTransforms.hlsl"
		#include "Packages\com.unity.render-pipelines.universal\ShaderLibrary\ShaderVariablesFunctions.hlsl"

		struct a2v
		{
			float4 positionOS:POSITION;
			float3 normalOS:NORMAL;
			float4 tangentOS:TANGENT;
			float2 uv:TEXCOORD0;
		};

		struct v2f
		{
			float4 positionCS:SV_POSITION;
			float3 normalWS:TEXCOORD0;
			float3 tangentWS:TEXCOORD1;
			float3 bitangentWS:TEXCOORD2;
			float4 uv:TEXCOORD3;
			float2 positionSS:TEXCOORD4;
		};

		TEXTURE2D(_DistortNormal);
		SAMPLER(sampler_DistortNormal);

		//Camera在Opaque之后渲染出的color buffer ，需要在Pipeline.assets中开启 Opaque Texture
		TEXTURE2D(_CameraOpaqueTexture);
		SAMPLER(sampler_CameraOpaqueTexture);

		TEXTURE2D(_NoiseTex);
		SAMPLER(sampler_NoiseTex);
		
		CBUFFER_START(UnityPerMaterial)
		float4 _DistortNormal_ST;
		float4 _NoiseTex_ST;		
		float _DistortStrength;
		CBUFFER_END

		ENDHLSL
		
		Pass 
		{
			HLSLPROGRAM
			
			#pragma shader_feature _USENORMAL_ON _USENORMAL_OFF
			#pragma vertex vert
			#pragma fragment frag

			v2f vert(a2v a) 
			{
				v2f o = (v2f)0;
				o.normalWS = TransformObjectToWorldNormal(a.normalOS);
				o.tangentWS = normalize(TransformObjectToWorldDir(a.tangentOS.xyz) * a.tangentOS.w);
				o.bitangentWS = normalize(cross(o.normalWS,o.tangentWS));
				o.uv.xy = TRANSFORM_TEX(a.uv,_DistortNormal);
				o.uv.zw = TRANSFORM_TEX(a.uv,_NoiseTex);
				o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
				o.positionSS = ComputeScreenPos(o.positionCS / o.positionCS.w).xy ;

				return o;
			}

			
			float4 frag(v2f i) : SV_TARGET
			{
				float3x3 tbn = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);

				float2 distortUV = float2(0,0);

				#ifdef _USENORMAL_ON
					//使用法线的方向对UV叠加扰动，
					//也可以使用普通的NoiseTex,但像素密度需要足够大
					float3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_DistortNormal,sampler_DistortNormal,i.uv)).rgb;
					normalTex.xy *= _DistortStrength;
					normalTex.z =sqrt(1 - normalTex.x * normalTex.x - normalTex.y * normalTex.y);
					float3 normalWS = normalize(mul(tbn,normalTex));
					distortUV  = normalWS.xy * _DistortStrength;
				#else

					float3 noiseCol = SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex,i.uv.zw).rgb;
					noiseCol = (noiseCol-0.5f) * 2;

					distortUV = noiseCol.rg * _DistortStrength;
				#endif

				float3 cameraCol = SAMPLE_TEXTURE2D(_CameraOpaqueTexture,sampler_CameraOpaqueTexture,i.positionSS.xy + distortUV).rgb;

				return float4(cameraCol,1.0);
			}

			ENDHLSL
		}
	} 
}
