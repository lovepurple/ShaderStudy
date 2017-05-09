/*
	深度图的应用-交互高亮(护盾效果)
*/
Shader "Depth/InteractHightlight"
{

	Properties
	{
		_BaseColor("Base Color",Color)=(1,1,1,1)
		_MainTex("MainTex",2D)="white"{}
		_HightLightColor("Hightlight Color",Color) = (0.8,0.9,0,1)
		
		_RimPower("Rim Power",float) = 1
		_RimColor("Rim Color",Color) = (0.8,0.2,0.9,1)

		_HightLightWidth("Hightlight Width",Range(0,1)) = 0
		/*
		_FresnelPower("Fresnel Power",float) = 128
		_FresnelColor("Fresnel Color",Color) = (0.8,0.2,0.9,1)
		*/

	/*	_SpeedX("Speed X",Range(0,1)) = 0
		_SpeedY("Speed Y",Range(0,1)) = 0
		
	*/
	}

	SubShader
	{

		Tags
		{ 
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		Pass{

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"  
		#include "Lighting.cginc"

		float4 _BaseColor;

		sampler2D _MainTex;
		float4 _MainTex_ST;

		sampler2D _CameraDepthTexture;

		float4 _HightLightColor;
		float _HightLightDepth;

		float _FresnelPower;
		float4 _FresnelColor;

		float _RimPower;
		float4 _RimColor;

		float _HightLightWidth;

		struct appdata
		{
			float4 vertex:POSITION;
			float2 texcoord:TEXCOORD0; 
			float3 normal:NORMAL;
		};

		struct v2f
		{
			float4 vertex:SV_POSITION;
			float2 uv:TEXCOORD1;
			float4 vertexScreenPos:TEXCOORD0; 
			float4 worldPos:TEXCOORD3;
			float3 worldNormal:TEXCOORD2;
		};

		v2f vert(appdata v)
		{
			v2f o = (v2f)0;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			o.worldPos = mul(unity_ObjectToWorld,v.vertex);
			o.worldNormal = UnityObjectToWorldNormal(v.normal);
			o.vertexScreenPos = ComputeScreenPos(o.vertex);

			return o;
		}	

		float4 frag(v2f i):SV_Target
		{
			float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos.xyz));
			float3 viewDir =normalize( UnityWorldSpaceViewDir(i.worldPos.xyz));

			float3 H = normalize(lightDir + viewDir);
			//float NDotH = max(0,dot(H,normalize(i.worldNormal)));
			float NDotV = saturate(dot(viewDir,normalize(i.worldNormal)));

			//Rim
			float4 col = pow((1-NDotV),_RimPower) * _RimColor;

			//depth operate
			float sceneDepth = Linear01Depth(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.vertexScreenPos)).r);
			float vertexZ = i.vertex.z;

			float diff = abs(sceneDepth - vertexZ) / _HightLightWidth;

			col.rgb = lerp(col,_HightLightColor,0).rgb;

			col.rgb = sceneDepth;
			col.a = 1;



			return col;

		}
	
		ENDCG
	}
	}

}