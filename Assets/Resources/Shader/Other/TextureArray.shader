/*
	Texture Array �Ǹ�������.asset��Դ����Ҫ�������� C#����Texture2DArray
*/
Shader "Others/TextureArray" {
	Properties{
		_TextureArr("Texture Array", 2DArray) = "" {}
		_SamplerTextureIndex("Sampler Index",float) = 1
	}
		SubShader{
			Tags {
				"RenderType" = "Opaque"
			}
			Pass {
				Name "FORWARD"
				Tags {
					"LightMode" = "ForwardBase"
				}


				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

			//����ʹ��3.5	����
			#pragma target 3.5

			float  _SamplerTextureIndex;

			UNITY_DECLARE_TEX2DARRAY(_TextureArr);		//�����Ǹ�2DArray

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv:TEXCOORD0;
			};
			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			float4 frag(VertexOutput i) : COLOR {
				float3 uv = float3(i.uv,floor(_SamplerTextureIndex));
				return UNITY_SAMPLE_TEX2DARRAY(_TextureArr, uv);			//UV��zֵ��ʾȡ���������ͼ

				}
				ENDCG
			}
		}
			FallBack "Diffuse"
}
