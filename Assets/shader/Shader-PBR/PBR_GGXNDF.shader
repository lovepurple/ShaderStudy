/**
	基于TrowBridge 的NDF 分布函数，现阶段用的最广泛的NDF函数（高光有长长的拖尾光） 

	a^2 / PI*d^2

	d = NDF^2 * (a^2 -1) +1

	 float a2 = roughness * roughness;
    float d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
    return UNITY_INV_PI * a2 / (d * d + 1e-7f)
**/
Shader "PBR/GGXNDF" 
{
	Properties 
	{
		_Smoothness("Smoothness",Range(0.00001,1)) = 0.1
	}

	SubShader 
	{ 
		Pass
		{
			Tags {"RenderType"="Opaque" "Queue"= "Geometry"}
			
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

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

			float GGXTerm (float NdotH, float roughness)
			{
				float square_roughness = roughness * roughness;
				float d = NdotH * NdotH * (square_roughness -1) + 1;
				float ggxTerm = square_roughness / (UNITY_PI * d * d);

				return ggxTerm;
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

				//float3 normalTex = UnpackNormal(tex2D(_NormalTex,i.uv.xy));
				
				// float3x3 tbnMatrix = float3x3(
				// float3(i.tangentWS.x,i.bitangentWS.x,i.normalWS.x),
				// float3(i.tangentWS.y,i.bitangentWS.y,i.normalWS.y),
				// float3(i.tangentWS.z,i.bitangentWS.z,i.normalWS.z)
				// );
				// float3 normalWS = mul(tbnMatrix,normalTex);
				float3 normalWS = i.normalWS;

				float3 halfDirWS = normalize(i.viewDirWS +i.lightDirWS);
				float HDV = max(0,dot(halfDirWS,i.viewDirWS));
				float NDL = max(0,dot(normalWS,i.lightDirWS));
				float NDH = max(0,dot(normalWS,halfDirWS));

				// float metallic = tex2D(_MetaTex,i.uv.xy).r;
				// float3 albedoCol = tex2D(_AlbedoTex,i.uv.xy);

				// float perceptualSmoothness = tex2D(_SmoothTex,i.uv.xy).r;
				// perceptualSmoothness = _Metallic;

				//输入
				float perceptualRoughness = 1 - _Smoothness;
				float roughness = perceptualRoughness * perceptualRoughness;


				//NDF 
				float NDF = GGXTerm(NDH,roughness);
				return float4(NDF,NDF,NDF,1.0);
			}
			
			ENDCG
		}
	}
}

