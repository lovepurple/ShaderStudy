/*
	通过Pass获取顶点正面和背面的Depth
*/
Shader "Unlit/ViewDepth"
{
	Properties
	{
	}
		//定义公共的用的是CGINCLUDE
		CGINCLUDE
		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
		
		struct v2f
		{
			float2 uv : TEXCOORD0;
			float3 viewPos : TEXCOORD1;
			float4 vertex : SV_POSITION;
		};

		struct ps_output_depth
		{
			float4 depth:SV_TARGET0;
		};
		
		v2f vert(appdata i) 
		{
			v2f o;
			o.uv = i.uv;
			o.vertex = UnityObjectToClipPos(i.vertex);
			o.viewPos = mul(UNITY_MATRIX_MV, i.vertex);

			return o;
		}

		ENDCG
		

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		//背面的深度
		Pass
		{
			Cull Front

			CGPROGRAM
			#pragma vert vert
			#pragma frag fragBackDepth

			ps_output_depth fragBackDepth(v2f i)
			{
				ps_output_depth o;
				o.depth = float4(0, i.viewPos.z, 0, 1);
			}


			ENDCG
				
			
		}

		//正面的深度
		Pass
		{
			Cull Back

			CGPROGRAM
			#pragma vert vert
			#pragma frag fragFrontDepth

				ps_output_depth fragFrontDepth(v2f i)
			{
				ps_output_depth o;
				o.depth = float4(i.viewPos.z,0, 0, 1);
			}


			ENDCG


		}
	}
}
