// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Billboard-Selfillum" {
	Properties
	{
		_MainTex("Main Texture",2D) = "white"{}
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
				#pragma target 3.0

				sampler2D _MainTex;
				float4 _MainTex_ST;

				uniform float4x4 _ObjectScaleMatrix;

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

					/*对于Billboard，就是个旋转算法，相机空间下，模型z一直朝向相机（模型的z恒定 = 模型中心点的z）
						1. mul(UNITY_MATRIX_MV,float4(0,0,0,1))  计算出的是模型原点（模型中心点）在相机空间下的坐标（所有的坐标都一样），所有的z值都一样
						2. mul(UNITY_MATRIX_MV,float4(0,0,0,1)) + float4(v.vertex.x,v.vertex.y,0,0)  由于v.vertex.xyz是模型坐标系（local position），
							所以坐标值都是相对模型坐标原点，因此1的结果计算出的是基于中心点的坐标加上本地坐标就是顶点在相机空间下的坐标
						3. mul(_ObjectScaleMatrix, float4(v.vertex.x, v.vertex.y, 0.0, 0.0)) 模型自身的缩放
					*/
					o.pos = mul(UNITY_MATRIX_P,  mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) + mul(_ObjectScaleMatrix, float4(v.vertex.x, v.vertex.y, 0.0, 0.0)));

					o.uv = v.uv;

					return o;

				}

				float4 frag(VertexOutput i) :SV_Target{
					return tex2D(_MainTex,i.uv);
				}

				ENDCG
			}

	}
		FallBack "Diffuse"
}
