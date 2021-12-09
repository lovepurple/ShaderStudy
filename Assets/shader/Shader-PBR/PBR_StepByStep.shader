/**
PBR 输入标准变量
Albedo 基础色（基础反射率）
Metalness 金属度
Roughness 粗糙度
**/
Shader "PBR/PBRStepByStep" 
{
	Properties 
	{
		_AlbedoTex("Albedo Tex",2D) = "white"{}
		_NormalTex("NormalMap",2D) ="bump"{}
		_MetaTex("Metal Tex(R)",2D) = "white"{}
		_SmoothTex("Smooth Tex(R)",2D)="white"{}

		_Smoothness("Smoothness" , Range(0.001,1)) = 0.1
	}

	SubShader 
	{ 
		Pass
		{
			//使用Lighting Probe时，必须加 "LightMode"="ForwardBase" 要不然黑
			Tags {"RenderType"="Opaque" "Queue"= "Geometry" "LightMode"="ForwardBase"}
			
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityStandardBRDF.cginc"

			sampler2D _AlbedoTex;
			sampler2D _NormalTex;
			sampler2D _SmoothTex;
			sampler2D _MetaTex;

			float _Metallic;
			float _Smoothness;

			struct a2v{
				float4 positionOS:POSITION;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;	
				float2 uv:TEXCOORD0;
			};

			struct v2f{
				float4 positionCS:SV_POSITION;
				float3 normalWS:TEXCOORD0;
				float3 viewDirWS:TEXCOORD1;
				float3 lightDirWS:TEXCOORD2;
				float4 positionWS:TEXCOORD3;
				float3 tangentWS:TEXCOORD4;
				float3 bitangentWS:TEXCOORD5;
				float2 uv:TEXCOORD6;
			};

			//Schlick 近似的Fresnel值
			float3 FresnelSchlickApproximate(float3 f0,float hDotL){
				return f0 + (1-f0)*pow( (1-hDotL),5);

			}

			float GGXNDF(float NDH,float roughness)
			{
				float a2 = roughness * roughness;
				float d = NDH * NDH * (a2 -1) + 1;
				float ggxValue = a2 / (UNITY_PI * d * d);

				return ggxValue;
			}

			/**
			GGX算法太多了。看引擎的实现吧。。。这里使用比较简单的一种 G = G(NDL) * G(NDV)
			*/
			float SmithVisibilityTerm(float NDLOrNDV,float k)
			{
				float g = NDLOrNDV / lerp(NDLOrNDV,1,k);
				return g;
			}

			/**

			**/
			float SmithGGX(float NDL,float NDV,float roughness)
			{
				//直接光照  Kdirect = (roughness + 1)^2 /8
				//间接光照  Kindrect = roughness * roughness /2
				float k = (roughness + 1) * (roughness + 1) / 8.0;
				float ggxL = SmithVisibilityTerm(NDL,k);
				float ggxV = SmithVisibilityTerm(NDV,k);
				return ggxL * ggxV;
			}

			v2f vert(a2v a){
				v2f o;
				o.positionCS = UnityObjectToClipPos(a.positionOS);
				o.positionWS = mul(unity_ObjectToWorld,a.positionOS);
				o.normalWS = UnityObjectToWorldNormal(a.normal);
				o.viewDirWS = normalize(UnityWorldSpaceViewDir(o.positionWS.xyz));
				o.lightDirWS =normalize(UnityWorldSpaceLightDir(o.positionWS.xyz));
				o.tangentWS = UnityObjectToWorldDir(a.tangent.xyz) * a.tangent.w;
				o.bitangentWS = cross(o.normalWS,o.tangentWS);
				o.uv = a.uv;
				
				return o;
			}

			//BlinnPhong NDF模型 
			float BlinnPhongNDF(float NDH,float perceptualRoughness)
			{
				float roughness2 = perceptualRoughness * perceptualRoughness;
				float roughness4 = roughness2 * roughness2;			// (2/ a2) -2
				float n = (2.0 / roughness4) -2;
				n = max(n,0.0001);			//避免是0

				// ( (n + 2) / 2 * PI ) * NDH ^ n
				float blinnPhongNDFVal = (n + 2)   * UNITY_INV_TWO_PI * pow(NDH,n);
				return blinnPhongNDFVal;
			}

			float4 frag(v2f i): SV_Target{

				float3 normalTex = UnpackNormal(tex2D(_NormalTex,i.uv.xy));
				
				float3x3 tbnMatrix = float3x3(
				float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				);
				float3 normalWS = mul(tbnMatrix,normalTex);

				float3 halfDirWS = normalize(i.viewDirWS +i.lightDirWS);
				float HDV = max(0,dot(halfDirWS,i.viewDirWS));
				float NDL = max(0,dot(normalWS,i.lightDirWS));
				float NDH = max(0,dot(normalWS,halfDirWS));
				float NDV = max(0,dot(normalWS,i.viewDirWS));

				float metallic = tex2D(_MetaTex,i.uv.xy).r;
				float3 albedoCol = tex2D(_AlbedoTex,i.uv.xy);

				float perceptualSmoothness = tex2D(_SmoothTex,i.uv.xy).r;
				perceptualSmoothness = _Smoothness;

				//输入
				float perceptualRoughness = 1 - perceptualSmoothness;
				float roughness = perceptualRoughness * perceptualRoughness;

				//计算真实的F0信息 
				//Fresnel
				float3 f0 = lerp(float3(0.04,0.04,0.04),albedoCol,metallic);
				float3 fresnelCol = FresnelSchlickApproximate(f0,HDV);
				// return float4(fresnelCol,1.0);


				//NDF 
				float NDF = GGXNDF(NDH,roughness);
				fresnelCol *= NDF;
				// return float4(fresnelCol,1.0);
				
				//GGX
				float ggx = SmithGGX(NDL,NDV,roughness);
				// return float4(ggx,ggx,ggx,1.0);

				float3 ks = fresnelCol;			//Ks = Fresnel 所以就不需要再乘ks
				float3 specularDirect = NDF * ggx * fresnelCol / (4 * NDL * NDV); 
				// return float4( specularDirect,1.0);

				float kd = (1-ks) *(1-metallic);
				float3 diffuseDirect = (kd * albedoCol / UNITY_PI);

				//直接光照 = （kd * albedo / pi  + DGF / 4 *ndl *ndv） * lightColor *ndl;
				float3 directCol = (specularDirect + diffuseDirect) * NDL;

				//间接光照部分，光照光照的漫反射基于球谐函数的采样 （输入normalWS）
				
				//采样周围的光(光照探针，天空盒啥的)
				half3 sh = ShadeSH9(float4(normalWS,1.0));
				float3 indirectKs = 
				return float4(sh.rgb,1.0);

				float3 finalCol = directCol;	
				// finalCol.rgb = HDV;


				// float N = GGXTerm(NDH,(1-_Smoothness));


				return float4(finalCol,1.0);
			}
			
			ENDCG
		}
	}
}

