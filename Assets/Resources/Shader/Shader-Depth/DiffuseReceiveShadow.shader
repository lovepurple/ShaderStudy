Shader "Shadow/DiffuseReceiveShadow" {
	Properties
	{
		_Color("Main Color",Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Main Tex",2D) = "white"{}
	}
		SubShader{
			Tags {
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
			}
			Pass {
				Name "FORWARD"
				Tags {
					"LightMode" = "ForwardBase"
					"RenderType" = "Transparent"
					"Queue" = "Transparent"
				}

				ZWrite Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#define UNITY_PASS_FORWARDBASE
				#include "UnityCG.cginc"
				#pragma multi_compile_fwdbase_fullshadows
				#pragma only_renderers d3d9 d3d11 glcore gles 
				#pragma target 3.0

				float4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;

				uniform sampler2D _ShadowMap;
				uniform float4x4 _LightSpaceProjectionUVMatrix;			//�ƹ�����ϵ�������UV,��������-> �ƹ�view -> �ƹ�Projection -> [-1,1]->[0,1]

				struct VertexInput {
					float4 vertex : POSITION;
					float2 uv:TEXCOORD0;
					float3 normal:NORMAL;			//Shadow acne��Ҫ�÷�����ƫ��
				};
				struct VertexOutput {
					float4 pos : SV_POSITION;
					float2 uv:TEXCOORD0;
					float4 worldPos:TEXCOORD1;
					float3 worldNormal:TEXCOORD2;
				};



					VertexOutput vert(VertexInput v) {
						VertexOutput o = (VertexOutput)0;
						o.worldPos = mul(unity_ObjectToWorld, v.vertex);
						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv = TRANSFORM_TEX(v.uv,_MainTex);
						o.worldNormal = UnityObjectToWorldNormal(v.normal);

						return o;
					}



					float4 frag(VertexOutput i) : COLOR {

						//��������Ӱ������µ�z������Ӱͼ�ϵ����  xy��ʾuv
						float3 posInShadowMap = mul(_LightSpaceProjectionUVMatrix,i.worldPos).xyz;			//ֱ�ӱȽ����
						float lightSpaceDepth = DecodeFloatRGBA(tex2D(_ShadowMap, posInShadowMap.xy));

						float bias = 0.005;

						//ֱ�Ӽ��˸�0.005   �ᵼ����ƫ�ƣ��˵�Ӱ ��㲻���Լ����£�
						float depth = posInShadowMap.z + bias;

						float4 baseCol = tex2D(_MainTex, i.uv);

						//Shadow with bias
						float3 worldSpaceLightDir = UnityWorldSpaceLightDir(i.worldPos);
						bias = max(0.05 * (1.0 - dot(i.worldNormal, worldSpaceLightDir)), 0.005);			//���߸����߽Ƕ�Խ��ƫ��Խ��
						depth = posInShadowMap.z + bias;

						float f = step(depth, lightSpaceDepth);

						return _Color*baseCol * (1 - f) + f *float4(0, 0, 0, 0.1);

					}
					ENDCG
					}
	}
		FallBack "Diffuse"
}
