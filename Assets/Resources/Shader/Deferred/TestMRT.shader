/*
	从Shader里获取 Diffuse Normal Depth Specular...
*/
Shader "Deferred Shading/TestMRT"
{
	Properties
	{
		_MainColor("Main Color",Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap("Bump Map",2D)="white"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal:TEXCOORD1;
				float3 worldTangent:TEXCOORD2;
				float3 worldBinormal:TEXCOORD3;
				float3 worldPos:TEXCOORD4;
			};

			//输出到RenderTexture 上，可以按4个通道存储
			//SV_Target后也有编号，表示输出的Buffer Index
			//如果camera.RendingPath = Deferred /Legency Deferred
			//	
			//			sampler2D _CameraGBufferTexture0;
			//			sampler2D _CameraGBufferTexture1;
			//			sampler2D _CameraGBufferTexture2;
			//			sampler2D _CameraGBufferTexture3;
			//可以直接采样到这四个G-Buffer 类似 _CameraDepthTexture的
			struct ps_output
			{
				float4 diffuse:SV_Target0;
				float4 worldNormal:SV_Target1;		//直接输出在屏幕上的是SV_Target0
				float4 worldPos:SV_Target2;
			};


			sampler2D _MainTex;
			float4 _MainTex_ST;

			//如果有多个贴图都需要用XX_ST的时候 需要每个都进行变换
			sampler2D _BumpMap;
			float4 _BumpMap_ST;

			float4 _MainColor;

			//采样深度图
			sampler2D _CameraDepthTexture;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _BumpMap);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);

				o.worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
				o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				o.worldBinormal = cross(o.worldNormal, o.worldTangent)*v.tangent.w;

				return o;
			}
			
			//输出Deferred 标准结构
			ps_output frag (v2f i)
			{
				ps_output o;
				o.diffuse = tex2D(_MainTex, i.uv.xy) * _MainColor;
				o.worldPos =float4(i.worldPos,1.0);

				//还原normal
				float3 normal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				float3x3 worldTBNMatrix = float3x3(i.worldTangent, i.worldBinormal, i.worldNormal);
				
				o.worldNormal = float4(normalize(mul(normal, worldTBNMatrix)), 1.0);

				////计算屏幕坐标采样深度图
				//float4 screenPos = ComputeScreenPos(i.worldPos);
				//screenPos.z = -

				return o;

			}
			ENDCG
		}
	}
}
