/*
	1. 基于LTC的面积光Shader,基于Deferred Shading
*/
Shader 'Hidden/Advanced Shading/AreaLight/Quad'
{
	Properties
	{
		_LightSourceWidth("Light Source Width",float) = 10
		_LightSourceHeight("Light Source Height",float) = 4

		_Intensity("Intensity",float) = 2
		_LightColor("Light Color",Color) = (1,1,1,1)

	}
	SubShader
	{
		CGPROGRAM
		#include 'UnityCG.cginc'
		#include "UnityPBSLighting.cginc"
		#include "UnityDeferredLibrary.cginc"

		//获取当前像素的世界坐标
		float3 getPixelWorldPosition()
		{

		}

		void getGBufferData(float2 uv,out float3 worldPosition,out float3 worldNormal,out float roughness)




		ENDCG



		pass
		{
			ZWrite Off
			Blend One One
			Cull Front
			ZTest Always

			CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag
			#include 'UnityCG.cginc'
			#include "UnityPBSLighting.cginc"
			#include "UnityDeferredLibrary.cginc"

			sampler2D _CameraGBufferTexture0;
			sampler2D _CameraGBufferTexture1;
			sampler2D _CameraGBufferTexture2;

			uniform sampler2D _ltc_diffuse_transform_inverse;
			uniform sampler2D _ltc_specular_transform_inverse;

			//光源坐标（使用中心计算quad 四个点坐标）
			uniform float3 _light_center_position;
			float _Intensity;
			float4 _LightColor;


		 	float4x3 quadVertex;


		 	//采样_GBufferTexture使用的uv是ScreenPos
			struct v2f_deferred
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeScreenPos(o.pos);
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return float4(0,0,0,0);
			}

			ENDCG
		}
	}
}