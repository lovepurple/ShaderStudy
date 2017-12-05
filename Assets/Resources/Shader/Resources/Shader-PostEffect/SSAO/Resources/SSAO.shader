/*
	SSAO Shader
*/
Shader "Hidden/PostEffect/SSAO" {
	Properties
	{
		//自遮蔽避因子
		_SelfOccusionFactor("Self Occusition Factor",Range(1,3)) = 1.5
	}


		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		ZWrite Off


		//SSAO pass
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DEBUG_SSAO_ON DEBUG_SSAO_OFF
			#pragma multi_compile USE_NOISETEXTURE_ON USE_NOISETEXTURE_OFF
			#pragma multi_compile USE_CUSTOM_COLOR_ON USE_CUSTOM_COLOR_OFF

			#include "UnityCG.cginc"

			uniform sampler2D _CameraDepthNormalsTexture;
			uniform sampler2D _CameraDepthTexture;

			//摄像机投影矩阵的逆矩阵
			uniform float4x4 _CameraProjectionMatrix_IT;

			//AO 强度
			uniform float _AOIntension;

			//AO 距离
			uniform float _Distance;

			//AO 采样半径
			uniform float _SampleRadius;

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;	   //Vector4(1 / width, 1 / height, width, height)

			//采样噪声图（随机出采样方向）
			uniform sampler2D _NoiseTexture;

			uniform float _NoiseTextureSize;

			uniform float4 _CustomColor;

			float _SelfOccusionFactor;


			//通过UV获取像素在ViewSpace中的位置
			inline float3 getPositionInViewSpace(float2 uv,out float depth)
			{
				depth = max(0,LinearEyeDepth(tex2D(_CameraDepthTexture,uv).r));

				float4 cameraRay = float4(uv * 2.0 - 1.0,1.0,1.0);

				cameraRay = mul(_CameraProjectionMatrix_IT,cameraRay);
				cameraRay.xyz /= cameraRay.w;

				return cameraRay.xyz * depth;
			}

			//获取ViewSpace中的法线
			inline float3 getNormalInViewSpace(float2 uv)
			{
				float3 viewSpaceNormalValue;
				float depth;

				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv), depth, viewSpaceNormalValue);

				return viewSpaceNormalValue;
			}

			//获取随机采样方向
			inline float2 getRandomDirection(float2 uv,float2 baseDirection)
			{
				#if USE_NOISETEXTURE_ON
				float2 randomNormal = normalize(tex2D(_NoiseTexture, uv).rg * 2.0 - 1.0);
				return reflect(baseDirection, randomNormal);
				#elif USE_NOISETEXTURE_OFF
				return baseDirection;
				#endif
			}

			inline float get01Depth(float eyeDepth)
			{
				return eyeDepth / (_ProjectionParams.z - _ProjectionParams.y);
			}

			//计算AO权值
			inline float calculateAOWeight(float2 sampleUV,float3 currentViewPos,float3 currentNormal)
			{
				//ViewSpace 下目标点的坐标
				float dstDepth = 0;
				float3 dstPosition = getPositionInViewSpace(sampleUV, dstDepth);
				float3 positionDiff = dstPosition - currentViewPos;

				//正上方的贡献度最大
				float intensity = max(0.0, dot(normalize(positionDiff), currentNormal));

				//距离衰减(这个函数有多种计算方程，一般都是倒数加常量)
				float distanceToDst = length(positionDiff) * _Distance;
				float attenuation = 1.0 / (_SelfOccusionFactor + distanceToDst);

				float aoWeight = intensity * attenuation * _AOIntension;

				return aoWeight;
			}

			inline float3 calculateSSAO(float2 uv)
			{
				//采样核
				const float2 sampleKernels[4] =
				{
					float2(0.0,1.0),		//Top
					float2(1.0,0.0),		//Right
					float2(0.0,-1.0),		//Down
					float2(-1.0,0.0)		//Left
				};

				//顺时针旋转45 旋转矩阵(sin 45 cos45 都近似于0.707)
				const float2x2 rotateMatrix = float2x2
					(
						float2(0.707,-0.707),
						float2(0.707,0.707)
					);

				float depth;

				//当前像素在viewSpace中的坐标
				float3 viewPosition = getPositionInViewSpace(uv, depth);

				//当前像素在viewSpace中的法线
				float3 viewNormal = getNormalInViewSpace(uv);

				//越远半径越小
				depth = get01Depth(depth);

				float radius = _SampleRadius * (1 - depth);		//要采样的半径(多少个像素)

				//convert to UV Space
				//float radiusInUVSpace = radius * min(_MainTex_TexelSize.x,_MainTex_TexelSize.y);

				float aoValue = 0.0;

				//对当前uv旁边的像素进行随机采样，计算AO的贡献度


				for (int i = 0; i < 1; ++i)
				{
					float2 sampleKernel = sampleKernels[i];
					float2 sampleUV = getRandomDirection(uv, sampleKernel) *radius *_MainTex_TexelSize.xy;
					sampleUV = getRandomDirection(uv,sampleKernel);
					return float3(sampleUV,0);

					//同时对要采样的uv旋转45度采样
					float2 kernelAfterRotate = mul(rotateMatrix, sampleUV);

					aoValue += calculateAOWeight(uv + sampleUV * 0.25, viewPosition, viewNormal);
					/*aoValue += calculateAOWeight(uv + kernelAfterRotate * 0.5, viewPosition, viewNormal);
					aoValue += calculateAOWeight(uv + sampleUV * 0.75, viewPosition, viewNormal);
					aoValue += calculateAOWeight(uv + kernelAfterRotate, viewPosition, viewNormal);*/

				}

			//	aoValue /= 16.0;

				aoValue = 1.0 - aoValue;

				return aoValue;
			}



			struct v2f
			{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				float4 screenPos:TEXCOORD1;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.pos);
				o.uv = v.texcoord;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1.0 - o.uv.y;
				#endif

				return o;
			}

			float4 frag(v2f i) :COLOR
			{

				float4 aoColor = (float4)1;
				aoColor.rgb = calculateSSAO(i.uv) ;
				return aoColor;
			}

			ENDCG
		}

		/*
		//Gaussian Blur Pass
		Pass
		{
			CGPROGRAM
			#pragma vertex vertBlur
			#pragma fragment fragBlur
			#include "UnityCG.cginc"

			uniform sampler2D _SSAOTexture;
			uniform float2 _BlurDirection;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
			};

			v2f vertBlur(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				float2 d1 = 1.3846153846 * _BlurDirection;
				float2 d2 = 3.2307692308 * _BlurDirection;
				o.uv1 = float4(o.uv + d1, o.uv - d1);
				o.uv2 = float4(o.uv + d2, o.uv - d2);
				return o;
			}

			float4 fragBlur(v2f i) : COLOR
			{
				#if USE_CUSTOM_COLOR_ON

				float3 c = tex2D(_MainTex, i.uv).rgb * 0.2270270270;
				c += tex2D(_MainTex, i.uv1.xy).rgb * 0.3162162162;
				c += tex2D(_MainTex, i.uv1.zw).rgb * 0.3162162162;
				c += tex2D(_MainTex, i.uv2.xy).rgb * 0.0702702703;
				c += tex2D(_MainTex, i.uv2.zw).rgb * 0.0702702703;
				return float4(c, 1.0);

				#else

				float c = tex2D(_MainTex, i.uv).r * 0.2270270270;
				c += tex2D(_MainTex, i.uv1.xy).r * 0.3162162162;
				c += tex2D(_MainTex, i.uv1.zw).r * 0.3162162162;
				c += tex2D(_MainTex, i.uv2.xy).r * 0.0702702703;
				c += tex2D(_MainTex, i.uv2.zw).r * 0.0702702703;
				return float4(c, c, c, 1.0);
				#endif
			}


			ENDCG
		}

		//final composite Pass
		Pass
		{
			CGPROGRAM

			#pragma vertex vertComposite
			#pragma fragment fragComposite
			#include "UnityCG.cginc"

			uniform sampler2D _SSAOTexture;
			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			v2f_img vertComposite(appdata_img i)
			{
				v2f_img o;

				o.uv = i.texcoord;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif

				o.pos = UnityObjectToClipPos(i.vertex);

				return o;
			}

			float4 fragComposite(v2f_img i) :COLOR
			{
				float3 aoColor = tex2D(_SSAOTexture,i.uv).rgb;
				float4 col = tex2D(_MainTex,i.uv);
				col.rgb *= aoColor;

				return col;
			}

			ENDCG

		}
			*/
	}
		FallBack "Diffuse"
}