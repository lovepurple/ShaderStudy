Shader "Unlit/OutlineWithMask"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white"{}
		_MainColor("Main Color",Color)=(1,1,1,1)
		_OutlineWidth("Outline Width",Range(0,0.02)) = 0.01
		_OutlineColor("Outline Color",Color) = (0.6,0.47,0.09,1.0)
	}
	SubShader
	{
		//先渲染边缘
		Pass
		{
			Name "Outline"

			Cull Front
			// ZWrite Off
			// ZTest Always
			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _OutlineColor;
			fixed _OutlineWidth;

			struct appdata
			{
				float4 vertex:POSITION;
				float3 normal:NORMAL;
			};

			struct v2f
			{	
				float4 pos:SV_POSITION;
			};

			v2f vert(appdata i)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP,i.vertex);
				float3 normalInViewSpace = mul((float3x3)UNITY_MATRIX_IT_MV,i.normal);
				float2 normalOffsetInProjection = TransformViewToProjection(normalInViewSpace.xy);

				o.pos.xy += normalOffsetInProjection * o.pos.z * _OutlineWidth;

				return o;
			}

			fixed4 frag(v2f i):COLOR
			{
				return _OutlineColor;
			}


			ENDCG

		}
		
		Pass
		{
			Name "Base"
			Tags{"Queue" = "Transparent"}
			ZWrite Off
			ZTest Always
			Blend DstColor Zero
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			sampler2D _MainTex;
			fixed4 _MainColor;

			//appdata_img 和v2f_img UnityCG.cginc 里的内置结构
			v2f_img vert(appdata_img i)
			{
				v2f_img o;
				o.pos = mul(UNITY_MATRIX_MVP,i.vertex);
				o.uv = i.texcoord;

				return o;
			}

			fixed4 frag(v2f_img i):COLOR
			{
				float4 finalColor = tex2D(_MainTex,i.uv) * _MainColor;
				return float4(1,1,1,0);
			}

			ENDCG
		}
		
	}
}
