Shader "URP/URP_DisplayVertexCoord"
{
	Properties
	{
		[KeywordEnum(None,VertexSpace,WorldSpace,ViewSpace,NDCSpace,PixelSpace,ScreenSpace)]_DisplayCoord("Display Coord",Float)=0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		#pragma shader_feature _DISPLAYCOORD_VERTEXSPACE _DISPLAYCOORD_WORLDSPACE _DISPLAYCOORD_VIEWSPACE _DISPLAYCOORD_NDCSPACE _DISPLAYCOORD_PIXELSPACE _DISPLAYCOORD_SCREENSPACE _DISPLAYCOORD_NONE

		struct a2v{
			float3 positionOS:POSITION;
		};

		struct v2f{
			float4 positionCS:SV_POSITION;
			float3 positionWS:TEXCOORD0;
			float3 positionVS:TEXCOORD1;
			float4 positionSS:TEXCOORD2;
			float4 positionNDC:TEXCOORD3;
			float3 positionOS:TEXCOORD4;
			float4 positionCS1:TEXCOORD5;
		};

		ENDHLSL
		
		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(a2v a){
				VertexPositionInputs vertexPostions = GetVertexPositionInputs(a.positionOS);
				v2f o;
				o.positionCS = vertexPostions.positionCS;
				o.positionNDC = vertexPostions.positionNDC;
				o.positionSS = ComputeScreenPos(o.positionCS);
				o.positionWS = vertexPostions.positionWS;
				o.positionVS = vertexPostions.positionVS;
				o.positionOS = a.positionOS;
				o.positionCS1 = vertexPostions.positionCS;

				return o;
			}

			real4 frag(v2f o):SV_TARGET{
				float2 positionSS = o.positionSS.xy;
				positionSS = ComputeScreenPos(o.positionCS1).xy;

				#if _DISPLAYCOORD_VERTEXSPACE
					return real4(o.positionOS,1.0);
				#elif _DISPLAYCOORD_WORLDSPACE 
					return real4(o.positionWS,1.0);
				#elif _DISPLAYCOORD_VIEWSPACE
					return real4(-o.positionVS.zzz,1.0);
				#elif _DISPLAYCOORD_NDCSPACE
					return real4(o.positionNDC.xy,0,1.0);
				#elif _DISPLAYCOORD_PIXELSPACE
					return real4(o.positionSS.xy,0,1);
				#elif _DISPLAYCOORD_SCREENSPACE
					return real4(positionSS.xy / o.positionSS.w,0,1);
				#else 
					return real4(o.positionVS.xy * _ZBufferParams.w,0,1.0);
				#endif
			}

			ENDHLSL

		}
	}

}
