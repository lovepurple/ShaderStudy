/*
	测试Deferred 输出的GBuffer
*/
Shader "Deferred/TestGBufferOutput"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 3.0

			#pragma multi_compile _DebugGBuffer0_ON 
			#pragma multi_compile _DebugGBuffer0_OFF

			#pragma multi_compile _DebugGBuffer1
			#pragma multi_compile _DebugGBuffer2

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _CameraGBufferTexture0;
			sampler2D _CameraGBufferTexture1;
			sampler2D _CameraGBufferTexture2;


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col0 = tex2D(_CameraGBufferTexture0,i.uv);
				float4 col1 = tex2D(_CameraGBufferTexture1, i.uv);
				float4 col2 = tex2D(_CameraGBufferTexture2, i.uv);

#ifdef _DebugGBuffer0_OFF

					return float4(1,1,0,1);
#endif


			

				return col1;
			}
			ENDCG
		}
	}
}
