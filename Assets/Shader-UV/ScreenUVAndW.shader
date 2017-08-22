/*
	��Ļ�ռ� w�����
*/
Shader "BaseTheory/ScreenUVAndW"
{
	Properties
	{
		_MainTex("MainTex",2D) = "white"{}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque"  }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex; //Depth Text

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 screenPos:TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				/* ������Ļuv�ļ��㷽�������ʹ��uv��ԭ����λ�� �����Է���
				o.screenPos = o.pos.xyw;			//clipspace wֵ��������z
				o.screenPos.y *= _ProjectionParams.x;		//x is 1.0 (or �C1.0 if currently rendering with a flipped projection matrix), �Ƿ��Ƿ���
				*/
				//���ַ���һ��
				o.screenPos = o.pos / o.pos.w;
				o.screenPos.y *= _ProjectionParams.x;

				//ʹ��screenPos ��ps ��Ͳ���Ҫȥת��ԭ����
				o.screenPos = ComputeScreenPos(o.pos);
				o.screenPos /= o.screenPos.w;



				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float2 screenUV = i.screenPos.xy;
				float4 finalColor = tex2D(_MainTex, screenUV);

				return finalColor;
			}

			ENDCG
		}
	}
		FallBack "VertexLit"
}