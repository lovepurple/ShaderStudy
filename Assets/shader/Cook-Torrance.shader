// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "JQM/Cook-Torrance"
{
	Properties
	{
		_Color("Base Color",Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}

	_Roughness("Roughness",Range(0,1)) = 1
		_Fresnel("Fresnel",Range(0,1)) = 1
		_K("K",Range(0,1)) = 1

		_Environment("Environment", Cube) = "white"
	}


		SubShader
	{

		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		float4 _Color;
	//使用Unity定义的变量:灯光
	uniform float4 _LightColor0;


	float _Roughness;
	float _Fresnel;
	float _K;
	samplerCUBE _Environment;

	struct VertexOutput
	{
		float4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD1;
		float2 uv : TEXCOORD2;
		float3 normal:Normal;

	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

	VertexOutput vert(appdata_full v)
	{
		VertexOutput o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.normal = v.normal;

		return o;
	}

	fixed4 frag(VertexOutput i) : COLOR
	{
		//将法线转到世界空间:乘以变换矩阵的逆的转置
		//float3 normalWorld  = mul(_Object2World,i.normal);
		float3 normalWorld = mul(i.normal,unity_WorldToObject);

		//观察者
		float3 eyeDir = normalize(_WorldSpaceCameraPos - i.posWorld).xyz;

		//光源
		float3 lightDir = normalize(_WorldSpaceLightPos0).xyz;

		fixed4 col = tex2D(_MainTex, i.uv);

		///计算漫反射
		float3 diffuse = _Color;

		//计算天空盒
		float3 r = reflect(-eyeDir,normalWorld);
		float4 reflectiveColor = texCUBE(_Environment,r);



		//计算高光
		//float3 h = (eyeDir+lightDir)/2;
		//float3 r = normalize(reflect(-lightDir,normalWorld));
		//float3 specular = saturate(dot(lightDir,normalWorld))* _SpecularColor * pow(saturate(dot(r,eyeDir)),_SpecularPower);

		//计算Cook-Torrance高光
		float s;
		float ln = saturate(dot(lightDir,normalWorld));

		if (ln > 0.0)//在光照范围内
		{
			float3 h = normalize(eyeDir + lightDir);
			float nh = saturate(dot(normalWorld, h));
			float nv = saturate(dot(normalWorld, eyeDir));
			float vh = saturate(dot(eyeDir, h));

			//G项
			float nh2 = 2.0*nh;
			float g1 = (nh2*nv) / vh;
			float g2 = (nh2*ln) / vh;
			float g = min(1.0,min(g1,g2));

			//D项：beckmann distribution function
			float m2 = _Roughness*_Roughness;
			float r1 = 1.0 / (4.0 * m2 *pow(nh,4.0));
			float r2 = (nh*nh - 1.0) / (m2 * nh*nh);
			float roughness = r1*exp(r2);

			//F项
			float fresnel = pow(1.0 - vh,5.0);
			fresnel *= (1.0 - _Fresnel);
			fresnel += _Fresnel;
			s = saturate((fresnel*g*roughness) / (nv*ln*3.14));

			//reflectiveColor *= fresnel;
		}

		float3 final = _LightColor0*ln*(_K*diffuse*reflectiveColor * 2 + s*(1 - _K)*reflectiveColor * 2) + UNITY_LIGHTMODEL_AMBIENT.xyz;
		return float4(final,1);
	}
		ENDCG
	}
	}
}