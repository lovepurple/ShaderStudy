Shader "Shadow/DiffuseReceiveShadow" {
	Properties
	{
		_Color("Main Color",Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Main Tex",2D) = "white"{}
		_ShadowColor("Shadow Color",Color) = (0,0,0,0.6)
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
				Blend SrcAlpha OneMinusSrcAlpha

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

				float4 _ShadowColor;

				uniform sampler2D _ShadowMap;
				uniform float4x4 _LightSpaceProjectionUVMatrix;			//灯光坐标系下物体的UV,顶点世界-> 灯光view -> 灯光Projection -> [-1,1]->[0,1]

				struct VertexInput {
					float4 vertex : POSITION;
					float2 uv:TEXCOORD0;
					float3 normal:NORMAL;			//Shadow acne需要用法线做偏移
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

						//顶点在阴影摄像机下的z及在阴影图上的深度  xy表示uv
						float3 posInShadowMap = mul(_LightSpaceProjectionUVMatrix,i.worldPos).xyz;			//直接比较深度
						float lightSpaceDepth = DecodeFloatRGBA(tex2D(_ShadowMap, posInShadowMap.xy));

						float bias = 0.005;

						//直接加了个0.005   会导致有偏移（人的影 起点不在自己脚下） bias
						float depth = posInShadowMap.z + bias;

						float4 baseCol = tex2D(_MainTex, i.uv);

						//Shadow with bias
						float3 worldSpaceLightDir = UnityWorldSpaceLightDir(i.worldPos);
						bias = max(0.05 * (1.0 - dot(i.worldNormal, worldSpaceLightDir)), 0.005);			//视线跟法线角度越大，偏移越多
						depth = posInShadowMap.z + bias;

						float f = step(depth, lightSpaceDepth);

						return _Color*baseCol * (1 - f) + f *lerp(baseCol,_ShadowColor,_ShadowColor.a);

					}
					ENDCG
					}
	}
		FallBack "Diffuse"
}
